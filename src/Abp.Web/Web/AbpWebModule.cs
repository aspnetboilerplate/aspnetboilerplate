using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Abp.Auditing;
using Abp.Modules;
using Abp.Runtime.Session;
using Abp.Web.Session;
using Abp.Configuration.Startup;
using Abp.Web.Configuration;
using Abp.Web.Security.AntiForgery;
using Abp.Collections.Extensions;
using Abp.Dependency;

namespace Abp.Web
{
    /// <summary>
    /// This module is used to use ABP in ASP.NET web applications.
    /// </summary>
    [DependsOn(typeof(AbpWebCommonModule))]    
    public class AbpWebModule : AbpModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            IocManager.Register<IAbpAntiForgeryWebConfiguration, AbpAntiForgeryWebConfiguration>();
            IocManager.Register<IAbpWebLocalizationConfiguration, AbpWebLocalizationConfiguration>();
            IocManager.Register<IAbpWebModuleConfiguration, AbpWebModuleConfiguration>();
            
            Configuration.ReplaceService<IPrincipalAccessor, HttpContextPrincipalAccessor>(DependencyLifeStyle.Transient);
            Configuration.ReplaceService<IClientInfoProvider, WebClientInfoProvider>(DependencyLifeStyle.Transient);

            AddIgnoredTypes();
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());            
        }

        private void AddIgnoredTypes()
        {
            var ignoredTypes = new[]
            {
                typeof(HttpPostedFileBase),
                typeof(IEnumerable<HttpPostedFileBase>),
                typeof(HttpPostedFileWrapper),
                typeof(IEnumerable<HttpPostedFileWrapper>)
            };
            
            foreach (var ignoredType in ignoredTypes)
            {
                Configuration.Auditing.IgnoredTypes.AddIfNotContains(ignoredType);
                Configuration.Validation.IgnoredTypes.AddIfNotContains(ignoredType);
            }
        }
    }
}
