using System.Reflection;
using Abp.Dependency;
using Abp.Modules;
using Abp.Startup;
using Abp.Users.Dto;
using Taskever.Mapping;

namespace Taskever.Startup
{
    public class TaskeverAppModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            TaskeverDtoMapper.Map();
            UserDtosMapper.Map();
        }
    }
}