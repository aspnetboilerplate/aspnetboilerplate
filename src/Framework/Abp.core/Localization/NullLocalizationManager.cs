using System.Globalization;

namespace Abp.Localization
{
    public class NullLocalizationManager : ILocalizationManager
    {
        public static NullLocalizationManager Instance { get { return _instance; } }
        private static readonly NullLocalizationManager _instance = new NullLocalizationManager();

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
