using System;
using Abp.Collections;
using Newtonsoft.Json;

namespace Abp.WebHooks
{
    internal class WebHooksConfiguration : IWebHooksConfiguration
    {
        public TimeSpan TimeoutDuration { get; set; } = TimeSpan.FromSeconds(60);

        public int MaxSendAttemptCount { get; set; } = 5;

        public ITypeList<WebHookDefinitionProvider> Providers { get; }

        public JsonSerializerSettings JsonSerializerSettings { get; set; } = null;

        public WebHooksConfiguration()
        {
            Providers = new TypeList<WebHookDefinitionProvider>();
        }
    }
}
