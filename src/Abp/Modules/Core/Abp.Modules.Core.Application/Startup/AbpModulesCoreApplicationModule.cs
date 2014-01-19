using Abp.Modules;
using Abp.Startup.Dependency;
using Abp.Users;
using Abp.Users.Dto;

namespace Abp.Startup
{
    [AbpModule("Abp.Modules.Core.Application", Dependencies = new[] { "Abp.Modules.Core" })]
    public class AbpModulesCoreApplicationModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(new AbpCoreModuleApplicationDependencyInstaller());
            Mapper.Map();
        }
    }
}
