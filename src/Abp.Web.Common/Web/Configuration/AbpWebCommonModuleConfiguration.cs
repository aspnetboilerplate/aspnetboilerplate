using Abp.Web.Api.ProxyScripting.Configuration;
using Abp.Web.Security.AntiForgery;

namespace Abp.Web.Configuration
{
    internal class AbpWebCommonModuleConfiguration : IAbpWebCommonModuleConfiguration
    {
        public bool SendAllExceptionsToClients { get; set; }

        public IApiProxyScriptingConfiguration ApiProxyScripting { get; }

        public IAbpAntiForgeryConfiguration AntiForgery { get; }

        public IWebEmbeddedResourcesConfiguration EmbeddedResources { get; }

        public AbpWebCommonModuleConfiguration(
            IApiProxyScriptingConfiguration apiProxyScripting, 
            IAbpAntiForgeryConfiguration abpAntiForgery,
            IWebEmbeddedResourcesConfiguration embeddedResources)
        {
            ApiProxyScripting = apiProxyScripting;
            AntiForgery = abpAntiForgery;
            EmbeddedResources = embeddedResources;
        }
    }
}