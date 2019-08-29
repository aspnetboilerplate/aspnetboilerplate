using System;
using System.Collections.Generic;

namespace Abp.Localization.Dictionaries
{
    public abstract class LocalizationDictionaryProviderBase : ILocalizationDictionaryProvider
    {
        public string SourceName { get; private set; }

        public ILocalizationDictionary DefaultDictionary { get; protected set; }

        public IDictionary<string, ILocalizationDictionary> Dictionaries { get; private set; }

        protected LocalizationDictionaryProviderBase()
        {
            Dictionaries = new Dictionary<string, ILocalizationDictionary>();
        }

        public virtual void Initialize(string sourceName)
        {
            SourceName = sourceName;
        }

        public void Extend(ILocalizationDictionary dictionary)
        {
            //Add
            ILocalizationDictionary existingDictionary;
            if (!Dictionaries.TryGetValue(dictionary.CultureInfo.Name, out existingDictionary))
            {
                Dictionaries[dictionary.CultureInfo.Name] = dictionary;
                return;
            }

            //Override
            var localizedStrings = dictionary.GetAllStrings();
            foreach (var localizedString in localizedStrings)
            {
                existingDictionary[localizedString.Name] = localizedString.Value;
            }
        }

        protected void CommonInitialize(Func<LocalizationDictionary> localizationDictionaryFactory,
            string resourceName, string sourceName, string fileExtension)
        {
            var dictionary = localizationDictionaryFactory();
            if (Dictionaries.ContainsKey(dictionary.CultureInfo.Name))
            {
                throw new AbpInitializationException(sourceName + " source contains more than one dictionary for the culture: " + dictionary.CultureInfo.Name);
            }

            Dictionaries[dictionary.CultureInfo.Name] = dictionary;

            if (resourceName.EndsWith(sourceName + fileExtension))
            {
                if (DefaultDictionary != null)
                {
                    throw new AbpInitializationException("Only one default localization dictionary can be for source: " + sourceName);
                }

                DefaultDictionary = dictionary;
            }
        }
    }
}