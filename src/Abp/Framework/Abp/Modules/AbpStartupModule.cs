using System.Reflection;
using Abp.Dependency;
using Abp.Dependency.Conventions;
using Abp.Domain.Uow;
using Abp.Startup;

namespace Abp.Modules
{
    public class AbpStartupModule : AbpModule
    {
        public override void PreInitialize(IAbpInitializationContext context)
        {
            base.PreInitialize(context);
            IocManager.Instance.AddConventionalRegisterer(new BasicConventionalRegisterer());
            UnitOfWorkRegistrer.Initialize(context);
        }

        public override void Initialize(IAbpInitializationContext context)
        {
            base.Initialize(context);
            IocManager.Instance.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(),
                new ConventionalRegistrationConfig
                {
                    InstallInstallers = false
                });
        }
    }
}