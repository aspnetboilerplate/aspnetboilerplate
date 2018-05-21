using System.Collections.Generic;
using Abp.Web.Api.ProxyScripting.Configuration;
using Abp.Web.MultiTenancy;
using Abp.Web.Security.AntiForgery;

namespace Abp.Web.Configuration
{
    internal class AbpWebCommonModuleConfiguration : IAbpWebCommonModuleConfiguration
    {
        public bool SendAllExceptionsToClients { get; set; }

        public IApiProxyScriptingConfiguration ApiProxyScripting { get; }

        public IAbpAntiForgeryConfiguration AntiForgery { get; }

        public IWebEmbeddedResourcesConfiguration EmbeddedResources { get; }

        public IWebMultiTenancyConfiguration MultiTenancy { get; }

        public AbpWebCommonModuleConfiguration(
            IApiProxyScriptingConfiguration apiProxyScripting, 
            IAbpAntiForgeryConfiguration abpAntiForgery,
            IWebEmbeddedResourcesConfiguration embeddedResources, 
            IWebMultiTenancyConfiguration multiTenancy)
        {
            ApiProxyScripting = apiProxyScripting;
            AntiForgery = abpAntiForgery;
            EmbeddedResources = embeddedResources;
            MultiTenancy = multiTenancy;
        }
    }
}