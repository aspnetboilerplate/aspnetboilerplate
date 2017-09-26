using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Localization.Dictionaries;

namespace Abp.Localization
{
    /// <summary>
    /// Extends <see cref="ILocalizationDictionaryProvider"/> to add tenant and database based localization.
    /// </summary>
    public class MultiTenantLocalizationDictionaryProvider : ILocalizationDictionaryProvider
    {
        public ILocalizationDictionary DefaultDictionary
        {
            get { return GetDefaultDictionary(); }
        }

        public IDictionary<string, ILocalizationDictionary> Dictionaries
        {
            get { return GetDictionaries(); }
        }

        private readonly ConcurrentDictionary<string, ILocalizationDictionary> _dictionaries;

        private string _sourceName;

        private readonly ILocalizationDictionaryProvider _internalProvider;

        private readonly IIocManager _iocManager;
        private ILanguageManager _languageManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiTenantLocalizationDictionaryProvider"/> class.
        /// </summary>
        public MultiTenantLocalizationDictionaryProvider(ILocalizationDictionaryProvider internalProvider, IIocManager iocManager)
        {
            _internalProvider = internalProvider;
            _iocManager = iocManager;
            _dictionaries = new ConcurrentDictionary<string, ILocalizationDictionary>();
        }

        public void Initialize(string sourceName)
        {
            _sourceName = sourceName;
            _languageManager = _iocManager.Resolve<ILanguageManager>();
            _internalProvider.Initialize(_sourceName);
        }

        protected virtual IDictionary<string, ILocalizationDictionary> GetDictionaries()
        {
            var languages = _languageManager.GetLanguages();

            foreach (var language in languages)
            {
                _dictionaries.GetOrAdd(language.Name, s => CreateLocalizationDictionary(language));
            }

            return _dictionaries;
        }

        protected virtual ILocalizationDictionary GetDefaultDictionary()
        {
            var languages = _languageManager.GetLanguages();
            if (!languages.Any())
            {
                throw new AbpException("No language defined!");
            }

            var defaultLanguage = languages.FirstOrDefault(l => l.IsDefault);
            if (defaultLanguage == null)
            {
                throw new AbpException("Default language is not defined!");
            }

            return _dictionaries.GetOrAdd(defaultLanguage.Name, s => CreateLocalizationDictionary(defaultLanguage));
        }

        protected virtual IMultiTenantLocalizationDictionary CreateLocalizationDictionary(LanguageInfo language)
        {
            var internalDictionary =
                _internalProvider.Dictionaries.GetOrDefault(language.Name) ??
                new EmptyDictionary(CultureInfo.GetCultureInfo(language.Name));

            var dictionary =  _iocManager.Resolve<IMultiTenantLocalizationDictionary>(new
            {
                sourceName = _sourceName,
                internalDictionary = internalDictionary
            });

            return dictionary;
        }

        public virtual void Extend(ILocalizationDictionary dictionary)
        {
            //Add
            ILocalizationDictionary existingDictionary;
            if (!_internalProvider.Dictionaries.TryGetValue(dictionary.CultureInfo.Name, out existingDictionary))
            {
                _internalProvider.Dictionaries[dictionary.CultureInfo.Name] = dictionary;
                return;
            }

            //Override
            var localizedStrings = dictionary.GetAllStrings();
            foreach (var localizedString in localizedStrings)
            {
                existingDictionary[localizedString.Name] = localizedString.Value;
            }
        }
    }
}