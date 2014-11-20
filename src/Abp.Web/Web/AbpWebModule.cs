using System.Reflection;
using System.Web;
using Abp.Configuration.Startup;
using Abp.Localization.Sources.Xml;
using Abp.Modules;

namespace Abp.Web
{
    /// <summary>
    /// This module is used to use ABP in ASP.NET web applications.
    /// </summary>
    public class AbpWebModule : AbpModule
    {
		/// <inheritdoc/>
        public override void PreInitialize()
        {
            if (HttpContext.Current != null)
            {
                XmlLocalizationSource.RootDirectoryOfApplication = HttpContext.Current.Server.MapPath("~");                
            }
        }

		/// <inheritdoc/>
        public override void Initialize()
        { 
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            Configuration.Localization.RegisterXmlSource("AbpWeb", "Localization\\AbpWeb");
        }
    }
}
