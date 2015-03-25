using System.Reflection;
using Abp.Application.Navigation;
using Abp.Application.Services.Interceptors;
using Abp.Auditing;
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
        private AuditingInterceptorRegistrar _auditingInterceptorRegistrar;

        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());
            
            UnitOfWorkRegistrar.Initialize(IocManager);
            ApplicationServiceInterceptorRegistrar.Initialize(IocManager);

            _auditingInterceptorRegistrar = new AuditingInterceptorRegistrar(IocManager.Resolve<IAuditingConfiguration>(), IocManager); //TODO: may be injected!
            _auditingInterceptorRegistrar.Initialize();

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
            IocManager.RegisterIfNot<IUnitOfWork, NullUnitOfWork>(DependencyLifeStyle.Transient);
            IocManager.RegisterIfNot<IAuditingStore, SimpleLogAuditingStore>(DependencyLifeStyle.Transient);
        }
    }
}