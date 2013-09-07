using Abp.Modules.Core.Dependency.Installers;

namespace Abp.Modules.Core
{
    [AbpModule("Abp.Modules.Core.Web", Dependencies = new[] { "Abp.Data", "Abp.Web", "Abp.Modules.Core", "Abp.Modules.Core.Data" })]
    public class AbpCoreModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new AbpInstaller());
        }
    }
}
