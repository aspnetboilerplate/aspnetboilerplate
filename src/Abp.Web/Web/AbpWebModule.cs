using System.Reflection;
using System.Web;
using Abp.Localization.Sources.Xml;
using Abp.Modules;
using Abp.Web.Configuration;

namespace Abp.Web
{
    /// <summary>
    /// This module is used to use ABP in ASP.NET web applications.
    /// </summary>
    public class AbpWebModule : AbpModule
    {
        public override void PreInitialize()
        {
            if (HttpContext.Current != null)
            {
                XmlLocalizationSource.RootDirectoryOfApplication = HttpContext.Current.Server.MapPath("~");                
            }

            IocManager.Register<IAbpWebModuleConfiguration, AbpWebModuleConfiguration>();
        }

        public override void Initialize()
        { 
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            Configuration.Localization.Sources.Add(new XmlLocalizationSource("AbpWeb", "Localization\\AbpWeb"));
        }
    }
}
