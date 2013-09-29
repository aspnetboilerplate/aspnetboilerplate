using Abp.Modules.Core.Application.Services.Dto.Mappings;
using Abp.Modules.Core.Startup.Dependency;

namespace Abp.Modules.Core.Startup
{
    [AbpModule("Abp.Modules.Core.Application")]
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
