using System.Collections.Generic;
using Abp.Localization;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Used for localization configurations.
    /// </summary>
    public interface ILocalizationConfiguration
    {
        /// <summary>
        /// Used to set languages available for this application.
        /// </summary>
        IList<LanguageInfo> Languages { get; }

        /// <summary>
        /// List of localization sources.
        /// </summary>
        ILocalizationSourceList Sources { get; }

        /// <summary>
        /// Used to enable/disable localization system.
        /// Default: true.
        /// </summary>
        bool IsEnabled { get; set; }
    }
}