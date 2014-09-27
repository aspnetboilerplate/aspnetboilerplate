using Abp.Configuration;
using Abp.Dependency;

namespace Abp.Startup.Configuration
{
    /// <summary>
    /// This class is used to configure ABP and modules on startup.
    /// </summary>
    internal class AbpConfiguration : DictionayBasedConfig, IAbpConfiguration
    {
        /// <summary>
        /// Used to set localization configuration.
        /// </summary>
        public IAbpLocalizationConfiguration Localization { get; private set; }

        /// <summary>
        /// Used to configure modules.
        /// Modules can write extension methods to <see cref="AbpModuleConfigurations"/> to add module specific configurations.
        /// </summary>
        public IAbpModuleConfigurations Modules { get; private set; }

        /// <summary>
        /// Private constructor for singleton pattern.
        /// </summary>
        public AbpConfiguration()
        {
            Localization = new AbpLocalizationConfiguration(IocManager.Instance);
            Modules = new AbpModuleConfigurations(this);
        }

        private sealed class AbpModuleConfigurations : IAbpModuleConfigurations
        {
            public IAbpConfiguration AbpConfiguration { get; private set; }

            public AbpModuleConfigurations(IAbpConfiguration abpConfiguration)
            {
                AbpConfiguration = abpConfiguration;
            }
        }
    }
}