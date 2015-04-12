using Abp.Localization.Dictionaries;

namespace Abp.Localization.Sources
{
    /// <summary>
    /// Interface for a dictionary based localization source.
    /// </summary>
    public interface IDictionaryBasedLocalizationSource : ILocalizationSource
    {
        /// <summary>
        /// Extends the source with given dictionary.
        /// </summary>
        /// <param name="dictionary">Dictionary to extend the source</param>
        void Extend(ILocalizationDictionary dictionary);
    }
}