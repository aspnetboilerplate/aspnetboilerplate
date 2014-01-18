using Abp.Localization;
using Abp.Modules;
using Abp.Web.Localization;
using Castle.Windsor.Installer;

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

            initializationContext.IocContainer.Install(FromAssembly.This());
            LocalizationHelper.RegisterSource<AbpWebLocalizationSource>();
        }
    }
}
