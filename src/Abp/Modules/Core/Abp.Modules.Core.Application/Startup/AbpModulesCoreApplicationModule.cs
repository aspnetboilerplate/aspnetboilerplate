using Abp.Modules;
using Abp.Startup.Dependency;
using Abp.Users;
using Abp.Users.Dto;
using Castle.Windsor.Installer;

namespace Abp.Startup
{
    [AbpModule("Abp.Modules.Core.Application", Dependencies = new[] { "Abp.Modules.Core" })]
    public class AbpModulesCoreApplicationModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(FromAssembly.This());
            UserDtosMapper.Map();
        }
    }
}
