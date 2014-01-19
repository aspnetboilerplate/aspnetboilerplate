using Abp.Modules;
using Abp.Startup;
using Castle.Windsor.Installer;
using Taskever.Mapping;

namespace Taskever.Startup
{
    [AbpModule("Taskever.Application", Dependencies = new[] { "Abp.Modules.Core.Application" })]
    public class TaskeverAppModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            initializationContext.IocContainer.Install(FromAssembly.This());
            TaskeverDtoMapper.Map();
        }
    }
}