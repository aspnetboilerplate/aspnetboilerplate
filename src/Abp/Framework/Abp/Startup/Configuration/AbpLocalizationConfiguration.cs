using Abp.Localization.Sources;

namespace Abp.Startup.Configuration
{
    /// <summary>
    /// Used for localization configurations.
    /// </summary>
    public class AbpLocalizationConfiguration
    {
        /// <summary>
        /// Used to enable/disable localization system.
        /// Default: true.
        /// </summary>
        public bool IsEnabled { get; set; }

        internal AbpLocalizationConfiguration()
        {
            IsEnabled = true;
        }

        public void AddSource(ILocalizationSource source)
        {
            
        }
    }
}