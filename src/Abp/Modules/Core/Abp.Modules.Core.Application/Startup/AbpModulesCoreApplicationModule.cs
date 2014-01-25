using System.Reflection;
using Abp.Dependency;
using Abp.Modules;
using Abp.Users.Dto;

namespace Abp.Startup
{
    [AbpModule("Abp.Modules.Core.Application", Dependencies = new[] { "Abp.Modules.Core" })]
    public class AbpModulesCoreApplicationModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
            UserDtosMapper.Map();
        }
    }
}
