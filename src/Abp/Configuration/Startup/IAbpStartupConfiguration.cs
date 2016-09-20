using System;
using Abp.Application.Features;
using Abp.Auditing;
using Abp.BackgroundJobs;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus;
using Abp.Notifications;
using Abp.Runtime.Caching.Configuration;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Used to configure ABP and modules on startup.
    /// </summary>
    public interface IAbpStartupConfiguration : IDictionaryBasedConfig
    {
        /// <summary>
        /// Gets the IOC manager associated with this configuration.
        /// </summary>
        IIocManager IocManager { get; }

        /// <summary>
        /// Used to set localization configuration.
        /// </summary>
        ILocalizationConfiguration Localization { get; }

        /// <summary>
        /// Used to configure navigation.
        /// </summary>
        INavigationConfiguration Navigation { get; }

        /// <summary>
        /// Used to configure <see cref="IEventBus"/>.
        /// </summary>
        IEventBusConfiguration EventBus { get; }

        /// <summary>
        /// Used to configure auditing.
        /// </summary>
        IAuditingConfiguration Auditing { get; }

        /// <summary>
        /// Used to configure caching.
        /// </summary>
        ICachingConfiguration Caching { get; }

        /// <summary>
        /// Used to configure multi-tenancy.
        /// </summary>
        IMultiTenancyConfig MultiTenancy { get; }

        /// <summary>
        /// Used to configure authorization.
        /// </summary>
        IAuthorizationConfiguration Authorization { get; }

        /// <summary>
        /// Used to configure validation.
        /// </summary>
        IValidationConfiguration Validation { get; }

        /// <summary>
        /// Used to configure settings.
        /// </summary>
        ISettingsConfiguration Settings { get; }

        /// <summary>
        /// Gets/sets default connection string used by ORM module.
        /// It can be name of a connection string in application's config file or can be full connection string.
        /// </summary>
        string DefaultNameOrConnectionString { get; set; }

        /// <summary>
        /// Used to configure modules.
        /// Modules can write extension methods to <see cref="IModuleConfigurations"/> to add module specific configurations.
        /// </summary>
        IModuleConfigurations Modules { get; }

        /// <summary>
        /// Used to configure unit of work defaults.
        /// </summary>
        IUnitOfWorkDefaultOptions UnitOfWork { get; }

        /// <summary>
        /// Used to configure features.
        /// </summary>
        IFeatureConfiguration Features { get; }

        /// <summary>
        /// Used to configure background job system.
        /// </summary>
        IBackgroundJobConfiguration BackgroundJobs { get; }

        /// <summary>
        /// Used to configure notification system.
        /// </summary>
        INotificationConfiguration Notifications { get; }

        /// <summary>
        /// Used to replace a service type.
        /// Given <see cref="replaceAction"/> should register an implementation for the <see cref="type"/>.
        /// </summary>
        /// <param name="type">The type to be replaced.</param>
        /// <param name="replaceAction">Replace action.</param>
        void ReplaceService(Type type, Action replaceAction);

        /// <summary>
        /// Gets a configuration object.
        /// </summary>
        T Get<T>();
    }
}