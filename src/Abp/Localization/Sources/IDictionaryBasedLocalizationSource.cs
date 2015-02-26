using Abp.Localization.Dictionaries;

namespace Abp.Localization.Sources
{
    /// <summary>
    /// A dictionary based localization source reads localization strings from
    /// dictionaries (see <see cref="ILocalizationDictionary"/>).
    /// </summary>
    public interface IDictionaryBasedLocalizationSource : ILocalizationSource
    {
        /// <summary>
        /// Adds a new dictionary to the source.
        /// </summary>
        /// <param name="dictionary">Dictionary to add</param>
        /// <param name="isDefault">Is this dictionary the default one? Default directory is used when requested string can not found in specified culture</param>
        void AddDictionary(ILocalizationDictionary dictionary, bool isDefault = false);
    }
}