using System.Reflection;
using Abp.Dependency;
using Abp.Localization;
using Abp.Modules;
using Abp.Web.Localization;

namespace Abp.Startup.Web
{
    /// <summary>
    /// This module is used to use ABP in ASP.NET web applications.
    /// </summary>
    [AbpModule("Abp.Web")]
    public class AbpWebApiModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        { 
            base.Initialize(initializationContext);

            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            LocalizationHelper.RegisterSource<AbpWebLocalizationSource>();
        }
    }
}
