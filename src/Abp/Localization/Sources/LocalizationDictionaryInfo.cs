using Abp.Localization.Dictionaries;

namespace Abp.Localization.Sources
{
    /// <summary>
    /// Used to hold a <see cref="ILocalizationDictionary"/> with additional information.
    /// </summary>
    public class LocalizationDictionaryInfo
    {
        /// <summary>
        /// Reference to the dictionary.
        /// </summary>
        public ILocalizationDictionary Dictionary { get; private set; }

        /// <summary>
        /// Is this dictionary the default language?
        /// </summary>
        public bool IsDefault { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="LocalizationDictionaryInfo"/>.
        /// </summary>
        /// <param name="dictionary">Reference to the dictionary</param>
        /// <param name="isDefault">Is this dictionary the default language</param>
        public LocalizationDictionaryInfo(ILocalizationDictionary dictionary, bool isDefault = false)
        {
            Dictionary = dictionary;
            IsDefault = isDefault;
        }
    }
}