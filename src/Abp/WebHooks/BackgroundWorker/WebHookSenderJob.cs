using System;
using System.Transactions;
using Abp.BackgroundJobs;
using Abp.Dependency;

namespace Abp.Webhooks.BackgroundWorker
{
    public class WebhookSenderJob : BackgroundJob<WebhookSenderArgs>, ITransientDependency
    {
        private readonly IWebhooksConfiguration _webhooksConfiguration;
        private readonly IWebhookSubscriptionManager _webhookSubscriptionManager;
        private readonly IWebhookSendAttemptStore _webhookSendAttemptStore;
        private readonly IWebhookSender _webhookSender;

        public WebhookSenderJob(
            IWebhooksConfiguration webhooksConfiguration,
            IWebhookSubscriptionManager webhookSubscriptionManager,
            IWebhookSendAttemptStore webhookSendAttemptStore,
            IWebhookSender webhookSender)
        {
            _webhooksConfiguration = webhooksConfiguration;
            _webhookSubscriptionManager = webhookSubscriptionManager;
            _webhookSendAttemptStore = webhookSendAttemptStore;
            _webhookSender = webhookSender;
        }

        public override void Execute(WebhookSenderArgs args)
        {
            if (args.TryOnce)
            {
                try
                {
                    SendWebhook(args);
                }
                catch (Exception e)
                {
                    Logger.Warn("An error occured while sending webhook with try once.", e);
                    // ignored
                }
            }
            else
            {
                SendWebhook(args);
            }
        }

        private void SendWebhook(WebhookSenderArgs args)
        {
            if (args.WebhookEventId == default)
            {
                return;
            }

            if (args.WebhookSubscriptionId == default)
            {
                return;
            }

            if (!args.TryOnce)
            {
                var sendAttemptCount = _webhookSendAttemptStore.GetSendAttemptCount(args.TenantId, args.WebhookEventId, args.WebhookSubscriptionId);
                if (sendAttemptCount > _webhooksConfiguration.MaxSendAttemptCount)
                {
                    return;
                }
            }

            try
            {
                _webhookSender.SendWebhook(args);
            }
            catch (Exception)
            {
                if (!TryDeactivateSubscriptionIfReachedMaxConsecutiveFailCount(args.TenantId, args.WebhookSubscriptionId))
                //no need to retry to send webhook since subscription disabled
                {
                    throw;//Throw exception to re-try sending webhook
                }
            }
        }

        private bool TryDeactivateSubscriptionIfReachedMaxConsecutiveFailCount(int? tenantId, Guid subscriptionId)
        {
            if (!_webhooksConfiguration.IsAutomaticSubscriptionDeactivationEnabled)
            {
                return false;
            }

            var hasXConsecutiveFail = _webhookSendAttemptStore
                .HasXConsecutiveFail(
                    tenantId,
                    subscriptionId,
                    _webhooksConfiguration.MaxConsecutiveFailCountBeforeDeactivateSubscription
                );

            if (!hasXConsecutiveFail)
            {
                return false;
            }

            using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Required))
            {
                _webhookSubscriptionManager.ActivateWebhookSubscription(subscriptionId, false);
                uow.Complete();
                return true;
            }

        }
    }
}
