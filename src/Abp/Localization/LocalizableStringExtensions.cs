using System.Globalization;

namespace Abp.Localization
{
    public static class LocalizableStringExtensions
    {
        /// <summary>
        /// Localizes the string in current culture.
        /// </summary>
        /// <param name="localizableString">Localizable string instance</param>
        /// <param name="localizationManager">Localization manager</param>
        /// <returns>Localized string</returns>
        public static string Localize(this ILocalizableString localizableString, ILocalizationManager localizationManager)
        {
            return localizableString.Localize(new LocalizationContext(localizationManager));
        }

        /// <summary>
        /// Localizes the string in current culture.
        /// </summary>
        /// <param name="localizableString">Localizable string instance</param>
        /// <param name="localizationManager">Localization manager</param>
        /// <param name="culture">culture</param>
        /// <returns>Localized string</returns>
        public static string Localize(this ILocalizableString localizableString, ILocalizationManager localizationManager, CultureInfo culture)
        {
            return localizableString.Localize(new LocalizationContext(localizationManager), culture);
        }
    }
}