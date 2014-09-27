namespace Abp.Startup.Configuration
{
    /// <summary>
    /// Used to provide a way to configure modules.
    /// Create entension methods to this class to be used over <see cref="AbpConfiguration.Modules"/> object.
    /// </summary>
    public interface IAbpModuleConfigurations
    {
        /// <summary>
        /// Gets the ABP configuration object.
        /// </summary>
        IAbpConfiguration AbpConfiguration { get; }
    }
}