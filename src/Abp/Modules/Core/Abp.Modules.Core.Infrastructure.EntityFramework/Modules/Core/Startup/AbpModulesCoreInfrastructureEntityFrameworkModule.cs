using System;
using System.Reflection;
using Abp.Dependency;
using Abp.Startup;
using Abp.Startup.Infrastructure.EntityFramework;

namespace Abp.Modules.Core.Startup
{
    public class AbpModulesCoreInfrastructureEntityFrameworkModule : AbpModule
    {
        public override Type[] GetDependedModules()
        {
            return new[]
                   {
                       typeof (AbpEntityFrameworkModule)
                   };
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
