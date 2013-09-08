using Abp.Modules;
using Taskever.Services.Dto;

namespace Taskever.Startup
{
    [AbpModule("Taskever.Core", Dependencies = new[] { "Abp.Modules.Core" })]
    public class TaskeverCoreModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            DtoMapper.Map();
        }
    }
}