using System.Globalization;

namespace Abp.Localization
{
    /// <summary>
    /// Default/null implementation of <see cref="ILocalizationManager"/> interface.
    /// It implements Null object and Singleton patterns.
    /// </summary>
    public class NullLocalizationManager : ILocalizationManager
    {
        /// <summary>
        /// Gets singleton instance.
        /// </summary>
        public static NullLocalizationManager Instance { get { return InnerInstance; } }
        private static readonly NullLocalizationManager InnerInstance = new NullLocalizationManager();

        public string GetString(string name)
        {
            return name;
        }

        public string GetString(string name, string languageCode)
        {
            return name;
        }

        public string GetString(string name, CultureInfo culture)
        {
            return name;
        }
    }
}
