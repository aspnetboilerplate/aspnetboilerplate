using System.Reflection;
using Abp.Dependency;
using Abp.Modules;
using Abp.Startup;
using Taskever.Mapping;

namespace Taskever.Startup
{
    [AbpModule("Taskever.Application", Dependencies = new[] { "Abp.Modules.Core.Application" })]
    public class TaskeverAppModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            TaskeverDtoMapper.Map();
        }
    }
}