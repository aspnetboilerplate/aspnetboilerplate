using Abp.Web.Api.ProxyScripting.Configuration;
using Abp.Web.Security;

namespace Abp.Web.Configuration
{
    internal class AbpWebCommonModuleConfiguration : IAbpWebCommonModuleConfiguration
    {
        public bool SendAllExceptionsToClients { get; set; }

        public IApiProxyScriptingConfiguration ApiProxyScripting { get; }

        public ICsrfConfiguration Csrf { get; }

        public AbpWebCommonModuleConfiguration(IApiProxyScriptingConfiguration apiProxyScripting, ICsrfConfiguration csrf)
        {
            ApiProxyScripting = apiProxyScripting;
            Csrf = csrf;
        }
    }
}