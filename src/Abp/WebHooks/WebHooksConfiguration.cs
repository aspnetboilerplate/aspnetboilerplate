using System;
using Abp.Collections;
using Newtonsoft.Json;

namespace Abp.WebHooks
{
    internal class WebHooksConfiguration : IWebHooksConfiguration
    {
        public TimeSpan WebHookTimeout { get; set; } = TimeSpan.FromSeconds(60);

        public int MaxRepetitionCount { get; set; } = 5;

        public ITypeList<WebHookDefinitionProvider> Providers { get; }

        public JsonSerializerSettings JsonSerializerSettings { get; set; } = null;

        public WebHooksConfiguration()
        {
            Providers = new TypeList<WebHookDefinitionProvider>();
        }
    }
}
