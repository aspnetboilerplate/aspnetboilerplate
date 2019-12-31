using Abp.Collections;
using Abp.Dependency;
using Newtonsoft.Json;

namespace Abp.WebHooks
{
    internal class WebHooksConfiguration : IWebHooksConfiguration, ISingletonDependency
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
