using System.Reflection;
using Abp.Dependency;
using Abp.Dependency.Conventions;
using Abp.Domain.Uow;
using Abp.Events.Bus;

namespace Abp.Modules
{
    public class AbpStartupModule : AbpModule
    {
        public override void PreInitialize()
        {
            base.PreInitialize();

            IocManager.AddConventionalRegisterer(new BasicConventionalRegisterer());
            IocManager.IocContainer.Install(new EventBusInstaller(IocManager));
            UnitOfWorkRegistrer.Initialize(IocManager);
        }

        public override void Initialize()
        {
            base.Initialize();
            
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(),
                new ConventionalRegistrationConfig
                {
                    InstallInstallers = false
                });
        }
    }
}