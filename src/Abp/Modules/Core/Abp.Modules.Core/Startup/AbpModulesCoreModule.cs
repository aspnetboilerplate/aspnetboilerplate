using Abp.Modules.Core.Startup.Dependency;
using Abp.Startup;

namespace Abp.Modules.Core.Startup
{
    [AbpModule("Abp.Modules.Core")]
    public class AbpModulesCoreModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new AbpCoreModuleDependencyInstaller());
        }
    }
}
