using System;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;

namespace Abp.WebHooks.BackgroundWorker
{
    public class WebHookSenderJob : BackgroundJob<WebHookSenderInput>
    {
        private readonly IIocResolver _iocResolver;
        private readonly IBackgroundJobManager _backgroundJobManager;
        private readonly IWebHooksConfiguration _webHooksConfiguration;

        public WebHookSenderJob(
            IIocResolver iocResolver,
            IBackgroundJobManager backgroundJobManager,
            IWebHooksConfiguration webHooksConfiguration)
        {
            _iocResolver = iocResolver;
            _backgroundJobManager = backgroundJobManager;
            _webHooksConfiguration = webHooksConfiguration;
        }

        public override void Execute(WebHookSenderInput args)
        {
            if (args.WebHookId == default)
            {
                throw new ArgumentNullException(nameof(args.WebHookId));
            }

            if (args.WebHookSubscriptionId == default)
            {
                throw new ArgumentNullException(nameof(args.WebHookSubscriptionId));
            }

            using (var webHookSender = _iocResolver.ResolveAsDisposable<IWebHookSender>())
            {
                if (webHookSender.Object.TrySendWebHook(args))
                {
                    return;
                }

                using (var workItemStore = _iocResolver.ResolveAsDisposable<IWebHookWorkItemStore>())
                {
                    var repetitionCount = workItemStore.Object.GetRepetitionCount(args.WebHookId, args.WebHookSubscriptionId);

                    if (repetitionCount < _webHooksConfiguration.MaxRepetitionCount)
                    {
                        //try send again
                        _backgroundJobManager.Enqueue<WebHookSenderJob, WebHookSenderInput>(args);
                    }
                }
            }
        }
    }
}
