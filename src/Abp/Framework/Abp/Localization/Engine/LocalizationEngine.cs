using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Abp.Exceptions;

namespace Abp.Localization.Engine
{
    internal class LocalizationEngine
    {
        private readonly Dictionary<string, LocalizationDictionary> _dictionaries;

        private LocalizationDictionary _defaultDictionary;

        public LocalizationEngine()
        {
            _dictionaries = new Dictionary<string, LocalizationDictionary>();
        }

        public void AddDictionary(CultureInfo cultureInfo, LocalizationDictionary dictionary, bool isDefault = false)
        {
            _dictionaries[cultureInfo.Name] = dictionary;
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
            LocalizationDictionary originalDictionary;
            if (_dictionaries.TryGetValue(cultureCode, out originalDictionary))
            {
                string strOriginal;
                if (originalDictionary.TryGetValue(name, out strOriginal))
                {
                    return strOriginal;
                }
            }

            //Try to get from same language dictionary
            if (cultureCode.Length == 5)
            {
                var langCode = cultureCode.Substring(0, 2);
                LocalizationDictionary langDictionary;
                if (_dictionaries.TryGetValue(langCode, out langDictionary))
                {
                    string strLang;
                    if (langDictionary.TryGetValue(name, out strLang))
                    {
                        return strLang;
                    }
                }
            }

            //Try to get from default language
            if (_defaultDictionary == null)
            {
                throw new AbpException("Can not find '" + name + "' and no default language is defined!");
            }

            string strDefault;
            if (_defaultDictionary.TryGetValue(name, out strDefault))
            {
                return strDefault;
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
                throw new AbpException("No default language is defined!");
            }
            
            var list = new List<LocalizedString>(_defaultDictionary.Count);
            foreach (var name in _defaultDictionary.Keys)
            {
                list.Add(new LocalizedString(culture, name, GetString(name, culture)));
            }

            return list.ToImmutableList();
        }
    }
}