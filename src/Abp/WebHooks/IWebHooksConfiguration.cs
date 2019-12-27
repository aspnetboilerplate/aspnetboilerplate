using Abp.Collections;

namespace Abp.WebHooks
{
    public interface IWebHooksConfiguration
    {
        /// <summary>
        ///  How many times <see cref="IWebHookPublisher"/> will try resend webhook until gets HttpStatusCode.OK 
        /// </summary>
        int MaxRepetitionCount { get; set; }

        /// <summary>
        /// Notification providers.
        /// </summary>
        ITypeList<WebHookDefinitionProvider> Providers { get; }
    }
}
