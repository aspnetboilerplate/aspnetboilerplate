using System.Reflection;
using Abp.Application;
using Abp.Application.Interceptors;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus;

namespace Abp.Modules
{
    public class AbpStartupModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegisterer(new BasicConventionalRegistrar());
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