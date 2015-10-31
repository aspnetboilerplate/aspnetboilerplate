using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Threading;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Extensions;
using Abp.Logging;

namespace Abp.Localization.Dictionaries
{
    /// <summary>
    /// This class is used to build a localization source
    /// which works on memory based dictionaries to find strings.
    /// </summary>
    public class DictionaryBasedLocalizationSource : IDictionaryBasedLocalizationSource
    {
        /// <summary>
        /// Unique Name of the source.
        /// </summary>
        public string Name { get; private set; }

        private ILocalizationConfiguration _configuration;

        private readonly ILocalizationDictionaryProvider _dictionaryProvider;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="dictionaryProvider"></param>
        public DictionaryBasedLocalizationSource(string name, ILocalizationDictionaryProvider dictionaryProvider)
        {
            if (name.IsNullOrEmpty())
            {
                throw new ArgumentNullException("name");                
            }

            Name = name;

            if (dictionaryProvider == null)
            {
                throw new ArgumentNullException("dictionaryProvider");
            }

            _dictionaryProvider = dictionaryProvider;
        }

        /// <inheritdoc/>
        public virtual void Initialize(ILocalizationConfiguration configuration, IIocResolver iocResolver)
        {
            _configuration = configuration;
            _dictionaryProvider.Initialize(Name);
        }

        /// <inheritdoc/>
        public string GetString(string name)
        {
            return GetString(name, Thread.CurrentThread.CurrentUICulture);
        }

        /// <inheritdoc/>
        public string GetString(string name, CultureInfo culture)
        {
            var value = GetStringOrNull(name, culture);

            if (value == null)
            {
                var exceptionMessage = string.Format(
                    "Can not find '{0}' in localization source '{1}'!",
                    name, Name
                    );

                if (!_configuration.ReturnGivenTextIfNotFound)
                {
                    throw new AbpException(exceptionMessage);
                }

                LogHelper.Logger.Warn(exceptionMessage);

                return _configuration.WrapGivenTextIfNotFound
                    ? string.Format("[{0}]", name)
                    : name;
            }

            return value;
        }

        public string GetStringOrNull(string name, bool tryDefaults = true)
        {
            return GetStringOrNull(name, Thread.CurrentThread.CurrentUICulture, tryDefaults);
        }

        public string GetStringOrNull(string name, CultureInfo culture, bool tryDefaults = true)
        {
            var cultureCode = culture.Name;
            var dictionaries = _dictionaryProvider.Dictionaries;

            //Try to get from original dictionary (with country code)
            ILocalizationDictionary originalDictionary;
            if (dictionaries.TryGetValue(cultureCode, out originalDictionary))
            {
                var strOriginal = originalDictionary.GetOrNull(name);
                if (strOriginal != null)
                {
                    return strOriginal.Value;
                }
            }

            //Try to get from same language dictionary (without country code)
            if (cultureCode.Length == 5) //Example: "tr-TR" (length=5)
            {
                var langCode = cultureCode.Substring(0, 2);
                ILocalizationDictionary langDictionary;
                if (dictionaries.TryGetValue(langCode, out langDictionary))
                {
                    var strLang = langDictionary.GetOrNull(name);
                    if (strLang != null)
                    {
                        return strLang.Value;
                    }
                }
            }

            //Try to get from default language
            var defaultDictionary = _dictionaryProvider.DefaultDictionary;
            if (defaultDictionary == null)
            {
                return null;
            }

            var strDefault = defaultDictionary.GetOrNull(name);
            if (strDefault == null)
            {
                return null;
            }

            return strDefault.Value;
        }

        /// <inheritdoc/>
        public IReadOnlyList<LocalizedString> GetAllStrings(bool includeDefaults = true)
        {
            return GetAllStrings(Thread.CurrentThread.CurrentUICulture, includeDefaults);
        }

        /// <inheritdoc/>
        public IReadOnlyList<LocalizedString> GetAllStrings(CultureInfo culture, bool includeDefaults = true)
        {
            var dictionaries = _dictionaryProvider.Dictionaries;

            //Create a temp dictionary to build
            var allStrings = new Dictionary<string, LocalizedString>();

            if (includeDefaults)
            {
                //Fill all strings from default dictionary
                var defaultDictionary = _dictionaryProvider.DefaultDictionary;
                if (defaultDictionary != null)
                {
                    foreach (var defaultDictString in defaultDictionary.GetAllStrings())
                    {
                        allStrings[defaultDictString.Name] = defaultDictString;
                    }
                }

                //Overwrite all strings from the language based on country culture
                if (culture.Name.Length == 5)
                {
                    ILocalizationDictionary langDictionary;
                    if (dictionaries.TryGetValue(culture.Name.Substring(0, 2), out langDictionary))
                    {
                        foreach (var langString in langDictionary.GetAllStrings())
                        {
                            allStrings[langString.Name] = langString;
                        }
                    }
                }
            }

            //Overwrite all strings from the original dictionary
            ILocalizationDictionary originalDictionary;
            if (dictionaries.TryGetValue(culture.Name, out originalDictionary))
            {
                foreach (var originalLangString in originalDictionary.GetAllStrings())
                {
                    allStrings[originalLangString.Name] = originalLangString;
                }
            }

            return allStrings.Values.ToImmutableList();
        }

        /// <summary>
        /// Extends the source with given dictionary.
        /// </summary>
        /// <param name="dictionary">Dictionary to extend the source</param>
        public virtual void Extend(ILocalizationDictionary dictionary)
        {
            //Add
            ILocalizationDictionary existingDictionary;
            if (!_dictionaryProvider.Dictionaries.TryGetValue(dictionary.CultureInfo.Name, out existingDictionary))
            {
                _dictionaryProvider.Dictionaries[dictionary.CultureInfo.Name] = dictionary;
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
