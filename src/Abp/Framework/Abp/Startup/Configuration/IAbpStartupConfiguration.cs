using System;
using Abp.Configuration;

namespace Abp.Startup.Configuration
{
    /// <summary>
    /// Used to configure ABP and modules on startup.
    /// </summary>
    public interface IAbpStartupConfiguration : IDictionaryBasedConfig
    {
        /// <summary>
        /// Used to set localization configuration.
        /// </summary>
        IAbpLocalizationConfiguration Localization { get; }

        /// <summary>
        /// Used to configure modules.
        /// Modules can write extension methods to <see cref="IAbpModuleConfigurations"/> to add module specific configurations.
        /// </summary>
        IAbpModuleConfigurations Modules { get; }
    }
}