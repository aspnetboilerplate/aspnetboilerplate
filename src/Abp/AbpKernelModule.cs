using System.Reflection;
using Abp.Application.Navigation;
using Abp.Application.Services;
using Abp.Auditing;
using Abp.Authorization;
using Abp.Authorization.Interceptors;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Localization;
using Abp.Modules;
using Abp.Net.Mail;
using Abp.Runtime.Validation.Interception;

namespace Abp
{
    public sealed class AbpKernelModule : AbpModule
    {
        private AuditingInterceptorRegistrar _auditingInterceptorRegistrar;

        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());

            ValidationInterceptorRegistrar.Initialize(IocManager);

            //TODO: Consider to change order of Uow and Auth interceptors..?
            UnitOfWorkRegistrar.Initialize(IocManager);
            AuthorizationInterceptorRegistrar.Initialize(IocManager);

            _auditingInterceptorRegistrar = new AuditingInterceptorRegistrar(IocManager.Resolve<IAuditingConfiguration>(), IocManager);
            _auditingInterceptorRegistrar.Initialize();

            Configuration.Auditing.Selectors.Add(
                new NamedTypeSelector(
                    "Abp.ApplicationServices",
                    type => typeof (IApplicationService).IsAssignableFrom(type)
                    )
                );

            Configuration.Settings.Providers.Add<EmailSettingProvider>();

            Configuration.UnitOfWork.RegisterFilter(AbpDataFilters.SoftDelete, true);
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
            IocManager.RegisterIfNot<IAuditInfoProvider, NullAuditInfoProvider>(DependencyLifeStyle.Transient);
            IocManager.RegisterIfNot<IAuditingStore, SimpleLogAuditingStore>(DependencyLifeStyle.Transient);
        }
    }
}