using System.Globalization;

namespace Abp.Localization
{
    public interface ILocalizationManager
    {
        string GetString(string name);

        string GetString(string name, string languageCode);

        string GetString(string name, CultureInfo culture);
    }
}
