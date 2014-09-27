namespace Abp.Startup.Configuration
{
    public interface IAbpConfiguration
    {
        /// <summary>
        /// Used to set localization configuration.
        /// </summary>
        IAbpLocalizationConfiguration Localization { get; }

        /// <summary>
        /// Used to configure modules.
        /// Modules can write extension methods to <see cref="AbpModuleConfigurations"/> to add module specific configurations.
        /// </summary>
        IAbpModuleConfigurations Modules { get; }

        /// <summary>
        /// Used to set a string named configuration.
        /// If there is already a configuration with same <see cref="name"/>, it's overwritten.
        /// </summary>
        /// <param name="name">Unique name of the configuration</param>
        /// <param name="value">Value of the configuration</param>
        void Set(string name, object value);

        /// <summary>
        /// Gets a configuration object with given name.
        /// </summary>
        /// <param name="name">Unique name of the configuration</param>
        /// <returns>Value of the configuration or null if not found</returns>
        object Get(string name);

        /// <summary>
        /// Gets a configuration object with given name.
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="name">Unique name of the configuration</param>
        /// <returns>Value of the configuration or null if not found</returns>
        T Get<T>(string name);

        /// <summary>
        /// Gets a configuration object with given name.
        /// </summary>
        /// <param name="name">Unique name of the configuration</param>
        /// <param name="defaultValue">Default value of the object if can not found given configuration</param>
        /// <returns>Value of the configuration or null if not found</returns>
        object Get(string name, object defaultValue);

        /// <summary>
        /// Gets a configuration object with given name.
        /// </summary>
        /// <typeparam name="T">Type of the object</typeparam>
        /// <param name="name">Unique name of the configuration</param>
        /// <param name="defaultValue">Default value of the object if can not found given configuration</param>
        /// <returns>Value of the configuration or null if not found</returns>
        T Get<T>(string name, T defaultValue);
    }
}