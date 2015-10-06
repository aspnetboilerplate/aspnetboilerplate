using Abp.Localization.Dictionaries;

namespace Abp.Localization.Sources
{
    /// <summary>
    /// Used to store a localization source extension information.
    /// </summary>
    public class LocalizationSourceExtensionInfo
    {
        /// <summary>
        /// Source name.
        /// </summary>
        public string SourceName { get; private set; }

        /// <summary>
        /// Extension dictionaries.
        /// </summary>
        public ILocalizationDictionaryProvider DictionaryProvider { get; private set; }

        /// <summary>
        /// Creates a new <see cref="LocalizationSourceExtensionInfo"/> object.
        /// </summary>
        /// <param name="sourceName">Source name</param>
        /// <param name="dictionaryProvider">Extension dictionaries</param>
        public LocalizationSourceExtensionInfo(string sourceName, ILocalizationDictionaryProvider dictionaryProvider)
        {
            SourceName = sourceName;
            DictionaryProvider = dictionaryProvider;
        }
    }
}