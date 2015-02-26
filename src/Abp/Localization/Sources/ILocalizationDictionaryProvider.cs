using Abp.Localization.Dictionaries;

namespace Abp.Localization.Sources
{
    /// <summary>
    /// Used to add localization dictionaries (<see cref="ILocalizationDictionary"/>)
    /// to a <see cref="IDictionaryBasedLocalizationSource"/>.
    /// </summary>
    public interface ILocalizationDictionaryProvider
    {
        /// <summary>
        /// This method should add dictionaries to given source.
        /// </summary>
        /// <param name="localizationSource">Localization source.</param>
        void AddDictionariesToLocalizationSource(IDictionaryBasedLocalizationSource localizationSource);
    }
}