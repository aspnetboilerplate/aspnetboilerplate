using System.Collections.Generic;

namespace Abp.Localization.Dictionaries
{
    /// <summary>
    /// Used to get localization dictionaries (<see cref="ILocalizationDictionary"/>)
    /// for a <see cref="IDictionaryBasedLocalizationSource"/>.
    /// </summary>
    public interface ILocalizationDictionaryProvider
    {
        ILocalizationDictionary DefaultDictionary { get; }

        IDictionary<string, ILocalizationDictionary> Dictionaries { get; }

        void Initialize(string sourceName);
        
        void Extend(ILocalizationDictionary dictionary);
    }
}