using System.Reflection;
using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using Abp.Web.Api.ProxyScripting.Configuration;
using Abp.Web.Api.ProxyScripting.Generators.JQuery;
using Abp.Web.Configuration;
using Abp.Web.Localization;

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
            IocManager.Register<IApiProxyScriptingConfiguration, ApiProxyScriptingConfiguration>();
            IocManager.Register<IAbpWebModuleConfiguration, AbpWebModuleConfiguration>();

            Configuration.Modules.AbpWeb().ApiProxyScripting.Generators[JQueryProxyScriptGenerator.Name] = typeof(JQueryProxyScriptGenerator);

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    AbpWebLocalizedMessages.SourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(), "Abp.Web.Common.Web.Localization.AbpWebXmlSource"
                        )));
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());            
        }
    }
}
