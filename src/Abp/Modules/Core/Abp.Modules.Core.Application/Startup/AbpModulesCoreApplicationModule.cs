using System;
using System.Reflection;
using Abp.Dependency;
using Abp.Modules;

namespace Abp.Startup
{
    public class AbpModulesCoreApplicationModule : AbpModule
    {
        protected override Type[] GetDependedModules()
        {
            return new[]
                   {
                       typeof(AbpModulesCoreModule)
                   };
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
