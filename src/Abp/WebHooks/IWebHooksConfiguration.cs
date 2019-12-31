using Abp.Collections;
using Abp.Json;
using Newtonsoft.Json;

namespace Abp.WebHooks
{
    public interface IWebHooksConfiguration
    {
        /// <summary>
        ///  How many times <see cref="IWebHookPublisher"/> will try resend webhook until gets HttpStatusCode.OK 
        /// </summary>
        int MaxRepetitionCount { get; set; }

        /// <summary>
        /// Json serializer settings for converting webhook data to json, If this is null default settings will be used. <see cref="JsonExtensions.ToJsonString(object,bool,bool)"/>
        /// </summary>
        JsonSerializerSettings JsonSerializerSettings { get; set; }

        /// <summary>
        /// Notification providers.
        /// </summary>
        ITypeList<WebHookDefinitionProvider> Providers { get; }
    }
}
