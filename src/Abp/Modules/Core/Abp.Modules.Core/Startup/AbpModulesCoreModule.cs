using Abp.Modules;
using Abp.Startup.Dependency;
using Abp.Startup.Dependency.Installers;

namespace Abp.Startup
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
