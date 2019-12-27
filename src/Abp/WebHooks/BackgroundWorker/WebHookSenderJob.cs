using Abp.BackgroundJobs;
using Abp.Dependency;

namespace Abp.WebHooks.BackgroundWorker
{
    public class WebHookSenderJob : BackgroundJob<WebHookSenderJobArgs>
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

        public override void Execute(WebHookSenderJobArgs args)
        {
            using (var webHookSender = _iocResolver.ResolveAsDisposable<IWebHookSender>())
            {
                if (webHookSender.Object.TrySendWebHook(args.WebHookId, args.WebHookSubscriptionId))
                {
                    return;
                }

                using (var workItemStore = _iocResolver.ResolveAsDisposable<IWebHookWorkItemStore>())
                {
                    var repetitionCount = workItemStore.Object.GetRepetitionCount(args.WebHookId, args.WebHookSubscriptionId);

                    if (repetitionCount < _webHooksConfiguration.MaxRepetitionCount)
                    {
                        //try send again
                        _backgroundJobManager.Enqueue<WebHookSenderJob, WebHookSenderJobArgs>(new WebHookSenderJobArgs()
                        {
                            WebHookId = args.WebHookId,
                            WebHookSubscriptionId = args.WebHookSubscriptionId
                        });
                    }
                }
            }
        }
    }
}
