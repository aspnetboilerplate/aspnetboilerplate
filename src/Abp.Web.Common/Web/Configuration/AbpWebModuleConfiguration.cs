using Abp.Web.Api.ProxyScripting.Configuration;

namespace Abp.Web.Configuration
{
    internal class AbpWebModuleConfiguration : IAbpWebModuleConfiguration
    {
        public bool SendAllExceptionsToClients { get; set; }

        public IApiProxyScriptingConfiguration ApiProxyScripting { get; }

        public AbpWebModuleConfiguration(IApiProxyScriptingConfiguration apiProxyScripting)
        {
            ApiProxyScripting = apiProxyScripting;
        }
    }
}