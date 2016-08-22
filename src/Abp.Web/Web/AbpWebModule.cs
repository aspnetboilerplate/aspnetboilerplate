using System.Reflection;
using System.Web;
using Abp.Localization.Sources.Xml;
using Abp.Modules;
using Abp.Runtime.Session;
using Abp.Web.Session;
using Abp.Configuration.Startup;

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
            if (HttpContext.Current != null)
            {
                XmlLocalizationSource.RootDirectoryOfApplication = HttpContext.Current.Server.MapPath("~");
            }

            Configuration.ReplaceService<IPrincipalAccessor, HttpContextPrincipalAccessor>();
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());            
        }
    }
}
