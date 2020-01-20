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
                catch (Exception)
                {
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
            if (args.WebhookId == default)
            {
                return;
            }

            if (args.WebhookSubscriptionId == default)
            {
                return;
            }

            if (!args.TryOnce)
            {
                var sendAttemptCount = _webhookSendAttemptStore.GetSendAttemptCount(args.TenantId, args.WebhookId, args.WebhookSubscriptionId);
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
                if (!DeactivateSubscriptionIfReachedMaxConsecutiveFailCount(args.TenantId, args.WebhookSubscriptionId))//no need to retry to send webhook since subscription disabled
                {
                    throw;
                }
            }
        }

        private bool DeactivateSubscriptionIfReachedMaxConsecutiveFailCount(int? tenantId, Guid subscriptionId)
        {
            if (!_webhooksConfiguration.IsAutomaticSubscriptionDeactivationEnabled)
            {
                return false;
            }

            var hasAnySuccessfulAttempt = _webhookSendAttemptStore
                .HasAnySuccessfulAttemptInLastXRecord(
                    tenantId,
                    subscriptionId,
                    _webhooksConfiguration.MaxConsecutiveFailCountBeforeDeactivateSubscription
                );

            if (!hasAnySuccessfulAttempt)
            {
                using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Required))
                {
                    _webhookSubscriptionManager.ActivateWebhookSubscription(subscriptionId, false);
                    uow.Complete();
                    return true;
                }
            }

            return false;
        }
    }
}
