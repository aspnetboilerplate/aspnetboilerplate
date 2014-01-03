using Abp.Localization;
using Abp.Modules;
using Abp.Startup;
using Abp.Web.Localization;

namespace Abp.Web.Startup
{
    [AbpModule("Abp.Web")]
    public class AbpWebApiModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        { 
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new LocalizationInstaller());
            LocalizationHelper.RegisterSource<AbpWebLocalizationSource>();
        }
    }
}
