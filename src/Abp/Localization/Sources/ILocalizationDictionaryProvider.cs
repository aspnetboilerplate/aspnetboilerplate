using System.Collections.Generic;
using Abp.Localization.Dictionaries;

namespace Abp.Localization.Sources
{
    /// <summary>
    /// Used to get localization dictionaries (<see cref="ILocalizationDictionary"/>)
    /// for a <see cref="DictionaryBasedLocalizationSource"/>.
    /// </summary>
    public interface ILocalizationDictionaryProvider
    {
        /// <summary>
        /// Gets all dictionaries for given source name.
        /// </summary>
        /// <param name="sourceName">Localization source name</param>
        /// <returns>Dictionaries for given source name</returns>
        IEnumerable<LocalizationDictionaryInfo> GetDictionaries(string sourceName);
    }
}