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
        /// <param name="sourceName"> </param>
        /// <param name="name">Key name</param>
        /// <returns>Localized string</returns>
        public static string GetString(string sourceName, string name)
        {
            return LocalizationManager.GetSource(sourceName).GetString(name);
        }

        /// <summary>
        /// Gets localized string for given key name and specified culture information.
        /// </summary>
        /// <param name="sourceName"> </param>
        /// <param name="name">Key name</param>
        /// <param name="culture">culture information</param>
        /// <returns>Localized string</returns>
        public static string GetString(string sourceName, string name, CultureInfo culture)
        {
            return LocalizationManager.GetSource(sourceName).GetString(name, culture);
        }

        /// <summary>
        /// Registers a localization source.
        /// </summary>
        /// <typeparam name="T">Type of the localization source.</typeparam>
        public static void RegisterSource<T>() where T : ILocalizationSource
        {
            LocalizationManager.RegisterSource(IocHelper.Resolve<T>());
        }
    }
}