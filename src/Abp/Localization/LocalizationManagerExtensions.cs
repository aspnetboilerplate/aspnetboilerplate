using System.Globalization;

namespace Abp.Localization
{
    public static class LocalizationManagerExtensions
    {
        /// <summary>
        /// Gets a localized string in current language.
        /// </summary>
        /// <returns>Localized string</returns>
        public static string GetString(this ILocalizationManager localizationManager, LocalizableString localizableString)
        {
            return localizationManager.GetString(localizableString.SourceName, localizableString.Name);
        }

        /// <summary>
        /// Gets a localized string in specified language.
        /// </summary>
        /// <returns>Localized string</returns>
        public static string GetString(this ILocalizationManager localizationManager, LocalizableString localizableString, CultureInfo culture)
        {
            return localizationManager.GetString(localizableString.SourceName, localizableString.Name, culture);
        }
    }
}