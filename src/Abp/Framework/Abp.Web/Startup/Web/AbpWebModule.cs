using System.Reflection;
using System.Web;
using Abp.Dependency;
using Abp.Localization;
using Abp.Localization.Sources.Xml;
using Abp.Modules;
using Abp.Web.Localization;

namespace Abp.Startup.Web
{
    /// <summary>
    /// This module is used to use ABP in ASP.NET web applications.
    /// </summary>
    public class AbpWebModule : AbpModule
    {
        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            XmlLocalizationSource.RootDirectoryOfApplication = HttpContext.Current.Server.MapPath("~");
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        { 
            base.Initialize(initializationContext);

            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            LocalizationHelper.RegisterSource<AbpWebLocalizationSource>();
        }
    }
}
