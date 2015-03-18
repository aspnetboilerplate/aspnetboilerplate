using System.Reflection;
using Abp.Application.Navigation;
using Abp.Application.Services.Interceptors;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Localization;
using Abp.Modules;
using Abp.Net.Mail;

namespace Abp
{
    public sealed class AbpKernelModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());
            UnitOfWorkRegistrar.Initialize(IocManager);
            ApplicationServiceInterceptorRegistrar.Initialize(IocManager);
            Configuration.Settings.Providers.Add<EmailSettingProvider>();
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
            RegisterMissingComponents();

            IocManager.Resolve<LocalizationManager>().Initialize();
            IocManager.Resolve<NavigationManager>().Initialize();
            IocManager.Resolve<PermissionManager>().Initialize();
            IocManager.Resolve<SettingDefinitionManager>().Initialize();
        }

        private void RegisterMissingComponents()
        {
            if (!IocManager.IsRegistered<IUnitOfWork>())
            {
                IocManager.Register<IUnitOfWork, NullUnitOfWork>(DependencyLifeStyle.Transient);
            }
        }
    }
}