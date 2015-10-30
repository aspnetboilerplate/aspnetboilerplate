using System.Collections.Generic;
using System.Globalization;
using Abp.Configuration.Startup;
using Abp.Dependency;

namespace Abp.Localization.Sources
{
    /// <summary>
    /// A Localization Source is used to obtain localized strings.
    /// </summary>
    public interface ILocalizationSource
    {
        /// <summary>
        /// Unique Name of the source.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// This method is called by ABP before first usage.
        /// </summary>
        void Initialize(ILocalizationConfiguration configuration, IIocResolver iocResolver);

        /// <summary>
        /// Gets localized string for given name in current language.
        /// Fallbacks to default language if not found in current culture.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        string GetString(string name);

        /// <summary>
        /// Gets localized string for given name and specified culture.
        /// Fallbacks to default language if not found in given culture.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        string GetString(string name, CultureInfo culture);

        /// <summary>
        /// Gets localized string for given name in current language.
        /// Fallbacks to default language if not found in given culture.
        /// Returns null if not found.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        string GetStringOrNull(string name);
        
        /// <summary>
        /// Gets localized string for given name and specified culture.
        /// Fallbacks to default language if not found in given culture.
        /// Returns null if not found.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        string GetStringOrNull(string name, CultureInfo culture);

        /// <summary>
        /// Gets all strings in current language.
        /// </summary>
        IReadOnlyList<LocalizedString> GetAllStrings();

        /// <summary>
        /// Gets all strings in specified culture.
        /// </summary>
        IReadOnlyList<LocalizedString> GetAllStrings(CultureInfo culture);
    }
}