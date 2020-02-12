using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using Abp.Web.Api.ProxyScripting.Configuration;
using Abp.Web.Api.ProxyScripting.Generators.JQuery;
using Abp.Web.Configuration;
using Abp.Web.MultiTenancy;
using Abp.Web.Security.AntiForgery;
using Abp.Reflection.Extensions;
using Abp.Web.Minifier;

namespace Abp.Web
{
    /// <summary>
    /// This module is used to use ABP in ASP.NET web applications.
    /// </summary>
    [DependsOn(typeof(AbpKernelModule))]    
    public class AbpWebCommonModule : AbpModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            IocManager.Register<IWebMultiTenancyConfiguration, WebMultiTenancyConfiguration>();
            IocManager.Register<IApiProxyScriptingConfiguration, ApiProxyScriptingConfiguration>();
            IocManager.Register<IAbpAntiForgeryConfiguration, AbpAntiForgeryConfiguration>();
            IocManager.Register<IWebEmbeddedResourcesConfiguration, WebEmbeddedResourcesConfiguration>();
            IocManager.Register<IAbpWebCommonModuleConfiguration, AbpWebCommonModuleConfiguration>();
            IocManager.Register<IJavaScriptMinifier, NUglifyJavaScriptMinifier>();

            Configuration.Modules.AbpWebCommon().ApiProxyScripting.Generators[JQueryProxyScriptGenerator.Name] = typeof(JQueryProxyScriptGenerator);

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    AbpWebConsts.LocalizaionSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(AbpWebCommonModule).GetAssembly(), "Abp.Web.Localization.AbpWebXmlSource"
                        )));
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(AbpWebCommonModule).GetAssembly());            
        }
    }
}
