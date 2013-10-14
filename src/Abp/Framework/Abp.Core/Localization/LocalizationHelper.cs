using System.Globalization;
using Abp.Dependency;

namespace Abp.Localization
{
    public static class LocalizationHelper
    {
        private static readonly ILocalizationManager LocalizationManager;

        static LocalizationHelper()
        {
            LocalizationManager = IocHelper.Resolve<ILocalizationManager>();
        }

        /// <summary>
        /// Gets localized string for given key name and current language.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        public static string GetString(string name)
        {
            return LocalizationManager.GetString(name);
        }

        /// <summary>
        /// Gets localized string for given key name and specified language.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="languageCode">Language</param>
        /// <returns>Localized string</returns>
        public static string GetString(string name, string languageCode)
        {
            return LocalizationManager.GetString(name, languageCode);            
        }

        /// <summary>
        /// Gets localized string for given key name and specified culture information.
        /// </summary>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        public static string GetString(string name, CultureInfo culture)
        {
            return LocalizationManager.GetString(name, culture);
        }
    }
}