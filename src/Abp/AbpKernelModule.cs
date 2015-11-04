using System;
using System.Reflection;
using Abp.Application.Features;
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
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.Net.Mail;
using Abp.Runtime.Caching;
using Abp.Runtime.Session;
using Abp.Runtime.Validation.Interception;

namespace Abp
{
    /// <summary>
    /// Kernel (core) module of the ABP system.
    /// No need to depend on this, it's automatically the first module always.
    /// </summary>
    public sealed class AbpKernelModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new BasicConventionalRegistrar());

            ValidationInterceptorRegistrar.Initialize(IocManager);

            FeatureInterceptorRegistrar.Initialize(IocManager);
            AuditingInterceptorRegistrar.Initialize(IocManager);

            UnitOfWorkRegistrar.Initialize(IocManager);

            AuthorizationInterceptorRegistrar.Initialize(IocManager);

            Configuration.Auditing.Selectors.Add(
                new NamedTypeSelector(
                    "Abp.ApplicationServices",
                    type => typeof(IApplicationService).IsAssignableFrom(type)
                    )
                );

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    AbpConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(), "Abp.Localization.Sources.AbpXmlSource"
                        )));

            Configuration.Settings.Providers.Add<LocalizationSettingProvider>();
            Configuration.Settings.Providers.Add<EmailSettingProvider>();
            
            Configuration.UnitOfWork.RegisterFilter(AbpDataFilters.SoftDelete, true);
            Configuration.UnitOfWork.RegisterFilter(AbpDataFilters.MustHaveTenant, true);
            Configuration.UnitOfWork.RegisterFilter(AbpDataFilters.MayHaveTenant, true);

            ConfigureCaches();
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

            IocManager.Resolve<SettingDefinitionManager>().Initialize();
            IocManager.Resolve<FeatureManager>().Initialize();
            IocManager.Resolve<NavigationManager>().Initialize();
            IocManager.Resolve<PermissionManager>().Initialize();
            IocManager.Resolve<LocalizationManager>().Initialize();
        }

        private void ConfigureCaches()
        {
            Configuration.Caching.Configure(AbpCacheNames.ApplicationSettings, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromHours(8);
            });

            Configuration.Caching.Configure(AbpCacheNames.TenantSettings, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(60);
            });

            Configuration.Caching.Configure(AbpCacheNames.UserSettings, cache =>
            {
                cache.DefaultSlidingExpireTime = TimeSpan.FromMinutes(20);
            });
        }

        private void RegisterMissingComponents()
        {
            IocManager.RegisterIfNot<IUnitOfWork, NullUnitOfWork>(DependencyLifeStyle.Transient);
            IocManager.RegisterIfNot<IAuditInfoProvider, NullAuditInfoProvider>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<IAuditingStore, SimpleLogAuditingStore>(DependencyLifeStyle.Transient);
            IocManager.RegisterIfNot<ITenantIdResolver, NullTenantIdResolver>(DependencyLifeStyle.Singleton);
            IocManager.RegisterIfNot<IAbpSession, ClaimsAbpSession>(DependencyLifeStyle.Singleton);
        }
    }
}