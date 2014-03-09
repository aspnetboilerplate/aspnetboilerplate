using System.Reflection;
using Abp.Dependency;
using Abp.Startup;

namespace Abp.Modules.Core.Mvc.Startup
{
    public class AbpModulesCoreWebMvcModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
