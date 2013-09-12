using Abp.Modules.Core.Services.Dto.Mappings;
using Abp.Modules.Core.Startup.Dependency;

namespace Abp.Modules.Core.Startup
{
    [AbpModule("Abp.Modules.Core")]
    public class AbpModulesCoreModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new AbpCoreModuleDependencyInstaller());
            DtoMapper.Map();
        }
    }
}
