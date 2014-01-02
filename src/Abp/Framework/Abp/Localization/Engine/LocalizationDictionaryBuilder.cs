using System.Globalization;

namespace Abp.Localization.Engine
{
    internal class LocalizationDictionaryBuilder
    {
        public static LocalizationDictionary BuildFromXmlString(string xmlString, CultureInfo cultureInfo)
        {
            var dictionary = new LocalizationDictionary();
            return dictionary;
        }
    }
}