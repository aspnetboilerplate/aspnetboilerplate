using System.Reflection;
using Abp.Application.Navigation;
using Abp.Application.Services.Interceptors;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Modules;

namespace Abp
{
    internal sealed class AbpCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());
            UnitOfWorkRegistrar.Initialize(IocManager);
            ApplicationServiceInterceptorRegistrar.Initialize(IocManager);
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

        public override void PostInitialize()
        {
            IocManager.Resolve<NavigationManager>().Initialize();
            IocManager.Resolve<PermissionManager>().Initialize();
            IocManager.Resolve<SettingDefinitionManager>().Initialize();
        }
    }
}