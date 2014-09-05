using System.Reflection;
using Abp.Dependency;
using Abp.Dependency.Conventions;
using Abp.Domain.Uow;
using Abp.Startup;

namespace Abp.Modules
{
    public class AbpStartupModule : AbpModule
    {
        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            IocManager.Instance.AddConventionalRegisterer(new BasicConventionalRegisterer());
            UnitOfWorkRegistrer.Initialize(initializationContext);
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(),
                                                             new ConventionalRegistrationConfig
                                                             {
                                                                 InstallInstallers = false
                                                             });
        }
    }
}