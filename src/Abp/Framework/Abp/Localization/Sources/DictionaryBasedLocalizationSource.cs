using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Abp.Exceptions;
using Abp.Localization.Dictionaries;

namespace Abp.Localization.Sources
{
    /// <summary>
    /// This class is used to build a localization source
    /// which works on memory based dictionaries to find strings.
    /// </summary>
    public class DictionaryBasedLocalizationSource : ILocalizationSource
    {
        /// <summary>
        /// Unique Name of the source.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// List of all dictionaries in this source.
        /// </summary>
        private readonly Dictionary<string, ILocalizationDictionary> _dictionaries;

        /// <summary>
        /// Default directory is used when requested string can not found in specified culture.
        /// </summary>
        private ILocalizationDictionary _defaultDictionary;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="name">Unique Name of the source</param>
        public DictionaryBasedLocalizationSource(string name)
        {
            Name = name;
            _dictionaries = new Dictionary<string, ILocalizationDictionary>();
        }

        /// <summary>
        /// Adds a new dictionary to the source.
        /// </summary>
        /// <param name="dictionary">Dictionary to add</param>
        /// <param name="isDefault">Is this dictionary the default one? Default directory is used when requested string can not found in specified culture</param>
        public void AddDictionary(ILocalizationDictionary dictionary, bool isDefault = false)
        {
            _dictionaries[dictionary.CultureInfo.Name] = dictionary;
            if (isDefault)
            {
                _defaultDictionary = dictionary;
            }
        }

        public string GetString(string name)
        {
            return GetString(name, Thread.CurrentThread.CurrentUICulture);
        }

        public string GetString(string name, CultureInfo culture)
        {
            var cultureCode = culture.Name;

            //Try to get from original dictionary
            ILocalizationDictionary originalDictionary;
            if (_dictionaries.TryGetValue(cultureCode, out originalDictionary))
            {
                var strOriginal = originalDictionary.GetOrNull(name);
                if (strOriginal != null)
                {
                    return strOriginal.Value;
                }
            }

            //Try to get from same language dictionary
            if (cultureCode.Length == 5)
            {
                var langCode = cultureCode.Substring(0, 2);
                ILocalizationDictionary langDictionary;
                if (_dictionaries.TryGetValue(langCode, out langDictionary))
                {
                    var strLang = langDictionary.GetOrNull(name);
                    if (strLang != null)
                    {
                        return strLang.Value;
                    }
                }
            }

            //Try to get from default language
            if (_defaultDictionary == null)
            {
                throw new AbpException("Can not find '" + name + "' and no default language is defined!");
            }

            var strDefault = _defaultDictionary.GetOrNull(name);
            if (strDefault != null)
            {
                return strDefault.Value;
            }

            throw new AbpException("Can not find '" + name + "'!");
        }

        public IReadOnlyList<LocalizedString> GetAllStrings()
        {
            return GetAllStrings(Thread.CurrentThread.CurrentUICulture);
        }

        public IReadOnlyList<LocalizedString> GetAllStrings(CultureInfo culture)
        {
            if (_defaultDictionary == null)
            {
                throw new AbpException("No default dictionary is defined!");
            }
            
            //Create a temp dictionary to build
            var dict = new Dictionary<string, LocalizedString>();

            //Fill all strings from default dictionary
            foreach (var defaultDictString in _defaultDictionary.GetAllStrings())
            {
                dict[defaultDictString.Name] = defaultDictString;
            }

            //Overwrite all strings from the language based on country culture
            if (culture.Name.Length == 5)
            {
                ILocalizationDictionary langDictionary;
                if (_dictionaries.TryGetValue(culture.Name.Substring(0, 2), out langDictionary))
                {
                    foreach (var langString in langDictionary.GetAllStrings())
                    {
                        dict[langString.Name] = langString;
                    }
                }
            }

            //Overwrite all strings from the original dictionary
            ILocalizationDictionary originalDictionary;
            if (_dictionaries.TryGetValue(culture.Name, out originalDictionary))
            {
                foreach (var originalLangString in originalDictionary.GetAllStrings())
                {
                    dict[originalLangString.Name] = originalLangString;
                }
            }

            return dict.Values.ToImmutableList();
        }
    }
}