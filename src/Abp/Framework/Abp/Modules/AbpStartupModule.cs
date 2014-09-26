using System.Reflection;
using Abp.Dependency;
using Abp.Dependency.Conventions;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Startup;

namespace Abp.Modules
{
    public class AbpStartupModule : AbpModule
    {
        public override void PreInitialize(IAbpInitializationContext context)
        {
            base.PreInitialize(context);

            IocManager.AddConventionalRegisterer(new BasicConventionalRegisterer());
            context.IocManager.IocContainer.Install(new EventBusInstaller(context.IocManager));
            UnitOfWorkRegistrer.Initialize(context);
        }

        public override void Initialize(IAbpInitializationContext context)
        {
            base.Initialize(context);
            
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(),
                new ConventionalRegistrationConfig
                {
                    InstallInstallers = false
                });
        }
    }
}