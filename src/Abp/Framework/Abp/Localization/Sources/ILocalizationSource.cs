using System.Collections.Generic;
using System.Globalization;

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
        /// Gets localized string for given name in current language.
        /// </summary>
        /// <param name="name">Name</param>
        /// <returns>Localized string</returns>
        string GetString(string name);

        /// <summary>
        /// Gets localized string for given name and specified culture.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        string GetString(string name, CultureInfo culture);

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