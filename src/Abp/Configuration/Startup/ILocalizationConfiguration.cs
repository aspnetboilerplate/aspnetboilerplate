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

        /// <summary>
        /// If this is set to true, the given text (name) is returned
        /// if not found in the localization source. That prevent exceptions if
        /// given name is not defined in the localization sources.
        /// Also writes a warning log.
        /// Default: true.
        /// </summary>
        bool ReturnGivenTextIfNotFound { get; set; }

        /// <summary>
        /// It returns the given text by wrapping with [ and ] chars
        /// if not found in the localization source.
        /// This is considered only if <see cref="ReturnGivenTextIfNotFound"/> is true.
        /// Default: true.
        /// </summary>
        bool WrapGivenTextIfNotFound { get; set; }

        /// <summary>
        /// It returns the given text by converting string from 'PascalCase' to a 'Sentense case'
        /// if not found in the localization source.
        /// This is considered only if <see cref="ReturnGivenTextIfNotFound"/> is true.
        /// Default: true.
        /// </summary>
        bool HumanizeTextIfNotFound { get; set; }

        /// <summary>
        /// Write (or not write) a warning log if given text can not found in the localization source.
        /// Default: true.
        /// </summary>
        bool LogWarnMessageIfNotFound { get; set; }
    }
}
