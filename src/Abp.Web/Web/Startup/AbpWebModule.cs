using System.Reflection;
using System.Web;
using Abp.Localization.Sources.Xml;
using Abp.Modules;
using Abp.Startup.Configuration;

namespace Abp.Web.Startup
{
    /// <summary>
    /// This module is used to use ABP in ASP.NET web applications.
    /// </summary>
    public class AbpWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            base.PreInitialize();

            if (HttpContext.Current != null)
            {
                XmlLocalizationSource.RootDirectoryOfApplication = HttpContext.Current.Server.MapPath("~");                
            }
        }

        public override void Initialize()
        { 
            base.Initialize();

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            //Configuration.Modules.AbpWeb().
            Configuration.Localization.RegisterXmlSource("AbpWeb", "Localization\\AbpWeb");
        }
    }
}
