using System;
using Abp.Collections;
using Abp.Json;
using Newtonsoft.Json;

namespace Abp.Webhooks
{
    public interface IWebhooksConfiguration
    {
        /// <summary>
        /// HttpClient timeout. Sender will wait <c>TimeoutDuration</c> second before throw timeout exception
        /// </summary>
        TimeSpan TimeoutDuration { get; set; }

        /// <summary>
        ///  How many times <see cref="IWebhookPublisher"/> will try resend webhook until gets HttpStatusCode.OK 
        /// </summary>
        int MaxSendAttemptCount { get; set; }

        /// <summary>
        /// Json serializer settings for converting webhook data to json, If this is null default settings will be used. <see cref="JsonExtensions.ToJsonString(object,bool,bool)"/>
        /// </summary>
        JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        /// Notification providers.
        /// </summary>
        ITypeList<WebhookDefinitionProvider> Providers { get; }
    }
}
