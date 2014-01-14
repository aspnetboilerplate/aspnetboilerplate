using Abp.Modules.Core.Application.Services.Dto.Mappings;
using Abp.Modules.Core.Startup.Dependency;
using Abp.Startup;

namespace Abp.Modules.Core.Startup
{
    [AbpModule("Abp.Modules.Core.Application", Dependencies = new[] { "Abp.Modules.Core" })]
    public class AbpModulesCoreApplicationModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new AbpCoreModuleApplicationDependencyInstaller());
            DtoMapper.Map();
        }
    }
}
