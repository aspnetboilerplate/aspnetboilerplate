using System;
using Abp.BackgroundJobs;
using Abp.Dependency;

namespace Abp.Webhooks.BackgroundWorker
{
    public class WebhookSenderJob : BackgroundJob<WebhookSenderArgs>, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly IWebhooksConfiguration _webhooksConfiguration;

        public WebhookSenderJob(
            IIocResolver iocResolver,
            IWebhooksConfiguration webhooksConfiguration)
        {
            _iocResolver = iocResolver;
            _webhooksConfiguration = webhooksConfiguration;
        }

        public override void Execute(WebhookSenderArgs args)
        {
            if (args.WebhookId == default)
            {
                throw new ArgumentNullException(nameof(args.WebhookId));
            }

            if (args.WebhookSubscriptionId == default)
            {
                throw new ArgumentNullException(nameof(args.WebhookSubscriptionId));
            }

            using (var webhookSender = _iocResolver.ResolveAsDisposable<IWebhookSender>())
            {
                using (var webhookSendAttemptStore = _iocResolver.ResolveAsDisposable<IWebhookSendAttemptStore>())
                {
                    var sendAttemptCount = webhookSendAttemptStore.Object.GetSendAttemptCount(args.TenantId, args.WebhookId, args.WebhookSubscriptionId);

                    if (sendAttemptCount > _webhooksConfiguration.MaxSendAttemptCount)
                    {
                        return;
                    }

                    webhookSender.Object.SendWebhook(args);
                }
            }
        }
    }
}
