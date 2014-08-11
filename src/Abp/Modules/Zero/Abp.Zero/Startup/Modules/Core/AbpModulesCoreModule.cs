using System.Reflection;
using Abp.Dependency;
using Abp.Modules;

namespace Abp.Startup.Modules.Core
{
    public class AbpModulesCoreModule : AbpModule
    {
        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
