using System.Collections.Generic;
using System.Reflection;
using System.Web;
using Abp.Localization.Sources.Xml;
using Abp.Modules;
using Abp.Runtime.Session;
using Abp.Web.Session;
using Abp.Configuration.Startup;
using Abp.Web.Configuration;
using Abp.Web.Security.AntiForgery;
using Abp.Collections.Extensions;

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
            IocManager.Register<IAbpWebModuleConfiguration, AbpWebModuleConfiguration>();
            
            if (HttpContext.Current != null)
            {
                XmlLocalizationSource.RootDirectoryOfApplication = HttpContext.Current.Server.MapPath("~");
            }

            Configuration.ReplaceService<IPrincipalAccessor, HttpContextPrincipalAccessor>();

            AddIgnoredTypes();
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());            
        }

        private void AddIgnoredTypes()
        {
            Configuration.Validation.IgnoredTypes.AddIfNotContains(typeof(HttpPostedFileBase));
            Configuration.Validation.IgnoredTypes.AddIfNotContains(typeof(IEnumerable<HttpPostedFileBase>));
            Configuration.Auditing.IgnoredTypes.AddIfNotContains(typeof(HttpPostedFileBase));
            Configuration.Auditing.IgnoredTypes.AddIfNotContains(typeof(IEnumerable<HttpPostedFileBase>));

            Configuration.Validation.IgnoredTypes.AddIfNotContains(typeof(HttpPostedFileWrapper));
            Configuration.Validation.IgnoredTypes.AddIfNotContains(typeof(IEnumerable<HttpPostedFileWrapper>));
            Configuration.Auditing.IgnoredTypes.AddIfNotContains(typeof(HttpPostedFileWrapper));
            Configuration.Auditing.IgnoredTypes.AddIfNotContains(typeof(IEnumerable<HttpPostedFileWrapper>));
        }
    }
}
