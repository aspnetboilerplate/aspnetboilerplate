using System.Collections.Generic;
using Abp.Dependency;
using Abp.Utils.Extensions.Collections;

namespace Abp.Startup.Configuration
{
    /// <summary>
    /// This class is used for configure ABP and modules on startup.
    /// </summary>
    internal class AbpConfiguration : IAbpConfiguration
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
        /// Gets/sets key-based configuration objects.
        /// </summary>
        /// <param name="name">Name of the setting</param>
        private object this[string name]
        {
            get { return _settings.GetOrDefault(name); }
            set { _settings[name] = value; }
        }

        /// <summary>
        /// Singletion instance.
        /// </summary>
        //public static AbpConfiguration Instance { get { return _instance; } }
        //private static readonly AbpConfiguration _instance = new AbpConfiguration();

        /// <summary>
        /// Additional string-based configurations.
        /// </summary>
        private readonly Dictionary<string, object> _settings;

        /// <summary>
        /// Private constructor for singleton pattern.
        /// </summary>
        public AbpConfiguration()
        {
            _settings = new Dictionary<string, object>();
            Localization = new AbpLocalizationConfiguration(IocManager.Instance);
            Modules = new AbpModuleConfigurations(this);
        }

        /// <summary>
        /// Used to set a string named configuration.
        /// If there is already a configuration with same <see cref="name"/>, it's overwritten.
        /// </summary>
        /// <param name="name">Unique name of the configuration</param>
        /// <param name="value">Value of the configuration</param>
        public void Set(string name, object value)
        {
            this[name] = value;
        }

        /// <summary>
        /// Gets a configuration object with given name.
        /// </summary>
        /// <param name="name">Unique name of the configuration</param>
        /// <returns>Value of the configuration or null if not found</returns>
        public object Get(string name)
        {
            return Get(name, null);
        }

        /// <summary>
        /// Gets a configuration object with given name.
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="name">Unique name of the configuration</param>
        /// <returns>Value of the configuration or null if not found</returns>
        public T Get<T>(string name)
        {
            return Get(name, default(T));
        }

        /// <summary>
        /// Gets a configuration object with given name.
        /// </summary>
        /// <param name="name">Unique name of the configuration</param>
        /// <param name="defaultValue">Default value of the object if can not found given configuration</param>
        /// <returns>Value of the configuration or null if not found</returns>
        public object Get(string name, object defaultValue)
        {
            var value = this[name];
            if (value == null)
            {
                return defaultValue;
            }

            return this[name];
        }

        /// <summary>
        /// Gets a configuration object with given name.
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="name">Unique name of the configuration</param>
        /// <param name="defaultValue">Default value of the object if can not found given configuration</param>
        /// <returns>Value of the configuration or null if not found</returns>
        public T Get<T>(string name, T defaultValue)
        {
            return (T)Get(name, (object)defaultValue);
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