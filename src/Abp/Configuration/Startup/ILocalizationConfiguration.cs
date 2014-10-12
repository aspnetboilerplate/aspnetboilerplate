using System.Collections.Generic;
using Abp.Localization;
using Abp.Localization.Sources;

namespace Abp.Configuration.Startup
{
    /// <summary>
    /// Used for localization configurations.
    /// </summary>
    public interface ILocalizationConfiguration
    {
        /// <summary>
        /// Used to enable/disable localization system.
        /// Default: true.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Used to set languages available for this application.
        /// </summary>
        List<LanguageInfo> Languages { get; }

        /// <summary>
        /// Adds a localization source.
        /// </summary>
        /// <param name="source">Localization source</param>
        void RegisterSource(ILocalizationSource source);
    }
}