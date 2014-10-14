using System.Collections.Generic;
using Abp.Localization;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Used for localization configurations.
    /// </summary>
    internal class LocalizationConfiguration : ILocalizationConfiguration
    {
        /// <summary>
        /// Used to set languages available for this application.
        /// </summary>
        public IList<LanguageInfo> Languages { get; private set; }

        /// <summary>
        /// List of localization sources.
        /// </summary>
        public ILocalizationSourceList Sources { get; private set; }

        /// <summary>
        /// Used to enable/disable localization system.
        /// Default: true.
        /// </summary>
        public bool IsEnabled { get; set; }

        public LocalizationConfiguration()
        {
            Languages = new List<LanguageInfo>();
            Sources = new LocalizationSourceList();
            IsEnabled = true;
        }
    }
}