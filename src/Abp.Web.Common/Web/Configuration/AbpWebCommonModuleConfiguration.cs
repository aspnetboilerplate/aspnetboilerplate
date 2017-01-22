using Abp.Web.Api.ProxyScripting.Configuration;
using Abp.Web.Security.AntiForgery;

namespace Abp.Web.Configuration
{
    internal class AbpWebCommonModuleConfiguration : IAbpWebCommonModuleConfiguration
    {
        public bool SendAllExceptionsToClients { get; set; }

        public IApiProxyScriptingConfiguration ApiProxyScripting { get; }

        public IAbpAntiForgeryConfiguration AntiForgery { get; }

        public AbpWebCommonModuleConfiguration(IApiProxyScriptingConfiguration apiProxyScripting, IAbpAntiForgeryConfiguration abpAntiForgery)
        {
            ApiProxyScripting = apiProxyScripting;
            AntiForgery = abpAntiForgery;
        }
    }
}