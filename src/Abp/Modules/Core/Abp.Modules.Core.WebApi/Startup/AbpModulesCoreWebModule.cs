using Abp.Modules.Core.Dependency.Installers;

namespace Abp.Modules.Core.Startup
{
    [AbpModule("Abp.Modules.Core.WebApi", Dependencies = new[] { "Abp.WebApi", "Abp.Modules.Core.Data" })]
    public class AbpModulesCoreWebApiModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new AbpCoreModuleWebApiInstaller());
        }
    }
}
