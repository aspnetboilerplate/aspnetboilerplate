using Abp.Dependency;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// This class is used to configure ABP and modules on startup.
    /// </summary>
    internal class AbpStartupConfiguration : DictionayBasedConfig, IAbpStartupConfiguration
    {
        //TODO@Halil: Register all properties to IOC and use over it!

        public IIocManager IocManager { get; private set; }

        /// <summary>
        /// Used to set localization configuration.
        /// </summary>
        public ILocalizationConfiguration Localization { get; private set; }

        /// <summary>
        /// Gets/sets default connection string used by ORM module.
        /// It can be name of a connection string in application's config file or can be full connection string.
        /// </summary>
        public string DefaultConnectionString { get; set; }

        /// <summary>
        /// Used to configure modules.
        /// Modules can write extension methods to <see cref="ModuleConfigurations"/> to add module specific configurations.
        /// </summary>
        public IModuleConfigurations Modules { get; private set; }

        /// <summary>
        /// Used to configure navigation.
        /// </summary>
        public INavigationConfiguration Navigation { get; set; }

        /// <summary>
        /// Private constructor for singleton pattern.
        /// </summary>
        public AbpStartupConfiguration(IIocManager iocManager)
        {
            IocManager = iocManager;
            Localization = new LocalizationConfiguration(iocManager);
            Modules = new ModuleConfigurations(this);
        }

        private sealed class ModuleConfigurations : IModuleConfigurations
        {
            public IAbpStartupConfiguration AbpConfiguration { get; private set; }

            public ModuleConfigurations(IAbpStartupConfiguration abpConfiguration)
            {
                AbpConfiguration = abpConfiguration;
            }
        }
    }
}