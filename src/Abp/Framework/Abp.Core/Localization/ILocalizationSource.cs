using System.Collections.Generic;
using System.Globalization;

namespace Abp.Localization
{
    /// <summary>
    /// This interface is used to obtain localized strings.
    /// </summary>
    public interface ILocalizationSource
    {
        string SourceName { get; }

        /// <summary>
        /// Gets localized string for given key name and current language.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        string GetString(string name);

        /// <summary>
        /// Gets localized string for given key name and specified culture information.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        string GetString(string name, CultureInfo culture);

        IList<string> GetAllStrings();

        IList<string> GetAllStrings(CultureInfo culture);
    }
}