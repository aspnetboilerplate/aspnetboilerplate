using System.Collections.Generic;
using Abp.Utils.Extensions.Collections;

namespace Abp.Startup.Configuration
{
    /// <summary>
    /// This class is used for ABP startup configuration.
    /// </summary>
    public class AbpConfiguration
    {
        /// <summary>
        /// Used to set localization configuration.
        /// </summary>
        public AbpLocalizationConfiguration Localization { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public AbpModuleConfigurations Modules { get; private set; }

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
        public static AbpConfiguration Instance { get { return _instance; } }
        private static readonly AbpConfiguration _instance = new AbpConfiguration();

        /// <summary>
        /// Additional string-based settings.
        /// </summary>
        private readonly Dictionary<string, object> _settings;

        /// <summary>
        /// Private constructor for singleton pattern.
        /// </summary>
        private AbpConfiguration()
        {
            _settings = new Dictionary<string, object>();
            Localization = new AbpLocalizationConfiguration();
            Modules = new AbpModuleConfigurations();
        }

        public void Set<T>(string name, T value)
        {
            this[name] = value;
        }

        public T Get<T>(string name)
        {
            return Get(name, default(T));
        }

        public T Get<T>(string name, T defaultValue)
        {
            var value = this[name];
            if (value == null)
            {
                return defaultValue;
            }

            return (T)this[name];
        }
    }
}