using Abp.Dependency;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// This class is used to configure ABP and modules on startup.
    /// </summary>
    internal class AbpStartupConfiguration : DictionayBasedConfig, IAbpStartupConfiguration
    {
        /// <summary>
        /// Used to set localization configuration.
        /// </summary>
        public IAbpLocalizationConfiguration Localization { get; private set; }

        /// <summary>
        /// Gets/sets default connection string used by ORM module.
        /// It can be name of a connection string in application's config file or can be full connection string.
        /// </summary>
        public string DefaultConnectionString { get; set; }

        /// <summary>
        /// Used to configure modules.
        /// Modules can write extension methods to <see cref="AbpModuleConfigurations"/> to add module specific configurations.
        /// </summary>
        public IAbpModuleConfigurations Modules { get; private set; }

        /// <summary>
        /// Private constructor for singleton pattern.
        /// </summary>
        public AbpStartupConfiguration()
        {
            Localization = new AbpLocalizationConfiguration(IocManager.Instance);
            Modules = new AbpModuleConfigurations(this);
        }

        private sealed class AbpModuleConfigurations : IAbpModuleConfigurations
        {
            public IAbpStartupConfiguration AbpConfiguration { get; private set; }

            public AbpModuleConfigurations(IAbpStartupConfiguration abpConfiguration)
            {
                AbpConfiguration = abpConfiguration;
            }
        }
    }
}