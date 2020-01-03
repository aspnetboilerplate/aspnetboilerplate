using System;
using System.Collections.Generic;

namespace Abp.WebHooks
{
    public class WebHookSenderInput
    {
        //Webhook information 

        /// <summary>
        /// <see cref="WebHookInfo"/> foreign id 
        /// </summary>
        public Guid WebHookId { get; set; }

        /// <summary>
        /// Webhook unique name
        /// </summary>
        public string WebHookDefinition { get; set; }

        /// <summary>
        /// WebHook data as JSON string.
        /// </summary>
        public string Data { get; set; }

        //Subscription information

        /// <summary>
        /// <see cref="WebHookSubscription"/> foreign id 
        /// </summary>
        public Guid WebHookSubscriptionId { get; set; }

        /// <summary>
        /// Subscription webhook endpoint
        /// </summary>
        public string WebHookUri { get; set; }

        /// <summary>
        /// User webhook secret
        /// </summary>
        public string Secret { get; set; }

        /// <summary>
        /// Gets a set of additional HTTP headers.That headers will be sent with the webhook.
        /// </summary>
        public IDictionary<string, string> Headers { get; set; }
    }
}
