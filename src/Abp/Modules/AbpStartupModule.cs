using System.Reflection;
using Abp.Application;
using Abp.Dependency.Conventions;
using Abp.Domain.Uow;
using Abp.Events.Bus;

namespace Abp.Modules
{
    public class AbpStartupModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegisterer(new BasicConventionalRegisterer());
            UnitOfWorkRegistrer.Initialize(IocManager);
            ApplicationLayerInterceptorRegisterer.Initialize(IocManager);
        }

        public override void Initialize()
        {
            base.Initialize();

            IocManager.IocContainer.Install(new EventBusInstaller(IocManager));

            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly(),
                new ConventionalRegistrationConfig
                {
                    InstallInstallers = false
                });
        }
    }
}