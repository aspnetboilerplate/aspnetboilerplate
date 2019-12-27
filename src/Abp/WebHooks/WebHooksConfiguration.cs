using Abp.Collections;
using Abp.Dependency;

namespace Abp.WebHooks
{
    internal class WebHooksConfiguration : IWebHooksConfiguration, ISingletonDependency
    {
        public ITypeList<WebHookDefinitionProvider> Providers { get; private set; }

        public WebHooksConfiguration()
        {
            Providers = new TypeList<WebHookDefinitionProvider>();
        }
    }
}
