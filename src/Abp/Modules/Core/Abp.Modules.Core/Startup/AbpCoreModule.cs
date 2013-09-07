using Abp.Modules.Core.Startup.Dependency;

namespace Abp.Modules.Core.Startup
{
    [AbpModule("Abp.Modules.Core")]
    public class AbpCoreModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new AbpCoreModuleDependencyInstaller());
            AutoMappingManager.Map();
        }
    }
}
