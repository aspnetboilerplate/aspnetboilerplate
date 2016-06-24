using System.Reflection;
using System.Web;
using Abp.Localization.Sources.Xml;
using Abp.Modules;

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
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());            
        }
    }
}
