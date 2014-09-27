using Abp.Localization.Sources;

namespace Abp.Startup.Configuration
{
    /// <summary>
    /// Used for localization configurations.
    /// </summary>
    public interface IAbpLocalizationConfiguration
    {
        /// <summary>
        /// Used to enable/disable localization system.
        /// Default: true.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Adds a localization source.
        /// </summary>
        /// <param name="source">Localization source</param>
        void RegisterSource(ILocalizationSource source);
    }
}