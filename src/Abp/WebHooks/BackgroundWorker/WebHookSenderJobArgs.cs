using System;

namespace Abp.WebHooks.BackgroundWorker
{
    public class WebHookSenderJobArgs
    {
        /// <summary>
        /// <see cref="WebHookInfo"/> foreign id 
        /// </summary>
        public Guid WebHookId { get; set; }

        /// <summary>
        /// <see cref="WebHookSubscription"/> foreign id 
        /// </summary>
        public Guid WebHookSubscriptionId { get; set; }
    }
}
