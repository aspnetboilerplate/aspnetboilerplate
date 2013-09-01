using System.Globalization;
using Abp.Localization;

namespace Taskever.Localization.Resources
{
    public class ResourcesLocalizationManager : ILocalizationManager
    {
        public string GetString(string name)
        {
            return AppTexts.ResourceManager.GetString(name);
        }

        public string GetString(string name, string languageCode)
        {
            return GetString(name, new CultureInfo(languageCode));
        }

        public string GetString(string name, CultureInfo culture)
        {
            return AppTexts.ResourceManager.GetString(name, culture);
        }
    }
}
