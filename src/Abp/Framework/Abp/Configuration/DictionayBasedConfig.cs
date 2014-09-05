using System;
using System.Collections.Generic;
using Abp.Utils.Extensions.Collections;

namespace Abp.Configuration
{
    /// <summary>
    /// Used to set/get custom configuration.
    /// </summary>
    public abstract class DictionayBasedConfig
    {
        /// <summary>
        /// Dictionary of custom configuration.
        /// </summary>
        protected Dictionary<string, object> Configs { get; private set; }

        /// <summary>
        /// Gets/sets a config value.
        /// Returns null if no config with given name.
        /// </summary>
        /// <param name="name">Name of the config</param>
        /// <returns>Value of the config</returns>
        public object this[string name]
        {
            get { return Configs.GetOrDefault(name); }
            set { Configs[name] = value; }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected DictionayBasedConfig()
        {
            Configs = new Dictionary<string, object>();
        }

        /// <summary>
        /// Gets a configuration value as a specific type.
        /// </summary>
        /// <param name="name">Name of the config</param>
        /// <typeparam name="T">Type of the config</typeparam>
        /// <returns>Value of the config (or null if not found)</returns>
        public T GetOrDefault<T>(string name)
        {
            var value = this[name];
            return value == null
                       ? default(T)
                       : (T) Convert.ChangeType(value, typeof (T));
        }
    }
}