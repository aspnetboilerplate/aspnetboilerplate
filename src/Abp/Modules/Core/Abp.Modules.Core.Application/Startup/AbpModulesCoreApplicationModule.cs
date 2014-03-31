using System.Reflection;
using Abp.Dependency;
using Abp.Modules;

namespace Abp.Startup
{
    public class AbpModulesCoreApplicationModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
