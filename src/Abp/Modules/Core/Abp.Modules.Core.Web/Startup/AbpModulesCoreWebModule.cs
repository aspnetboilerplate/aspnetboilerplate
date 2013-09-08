using Abp.Modules.Core.Dependency.Installers;

namespace Abp.Modules.Core.Startup
{
    [AbpModule("Abp.Modules.Core.Web", Dependencies = new[] { "Abp.Web", "Abp.Modules.Core.Data" })]
    public class AbpModulesCoreWebModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new AbpInstaller());
        }
    }
}
