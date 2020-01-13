using System;
using Abp.BackgroundJobs;
using Abp.Dependency;

namespace Abp.Webhooks.BackgroundWorker
{
    public class WebhookSenderJob : BackgroundJob<WebhookSenderArgs>, ITransientDependency
    {
        private readonly IIocResolver _iocResolver;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IWebhooksConfiguration _webhooksConfiguration;

        public WebhookSenderJob(
            IIocResolver iocResolver,
            IBackgroundJobManager backgroundJobManager,
            IWebhooksConfiguration webhooksConfiguration)
        {
            _iocResolver = iocResolver;
            _backgroundJobManager = backgroundJobManager;
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
                if (webhookSender.Object.TrySendWebhook(args))
                {
                    return;
                }

                using (var workItemStore = _iocResolver.ResolveAsDisposable<IWebhookSendAttemptStore>())
                {
                    var sendAttemptCount = workItemStore.Object.GetSendAttemptCount(args.TenantId, args.WebhookId, args.WebhookSubscriptionId);

                    if (sendAttemptCount < _webhooksConfiguration.MaxSendAttemptCount)
                    {
                        //try send again
                        _backgroundJobManager.Enqueue<WebhookSenderJob, WebhookSenderArgs>(args);
                    }
                }
            }
        }
    }
}
