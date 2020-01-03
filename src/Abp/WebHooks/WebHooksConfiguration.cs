using Abp.Collections;
using Newtonsoft.Json;

namespace Abp.WebHooks
{
    internal class WebHooksConfiguration : IWebHooksConfiguration
    {
        public ITypeList<WebHookDefinitionProvider> Providers { get; }

        public int MaxRepetitionCount { get; set; } = 5;

        public JsonSerializerSettings JsonSerializerSettings { get; set; } = null;

        public WebHooksConfiguration()
        {
            Providers = new TypeList<WebHookDefinitionProvider>();
        }
    }
}
