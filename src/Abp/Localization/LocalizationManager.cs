using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Threading;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Localization.Sources;
using Castle.Core.Logging;

namespace Abp.Localization
{
    internal class LocalizationManager : ILocalizationManager
    {
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets current language for the application.
        /// </summary>
        public LanguageInfo CurrentLanguage { get { return GetCurrentLanguage(); } }

        private readonly ILocalizationConfiguration _configuration;
        private readonly IIocResolver _iocResolver;
        private readonly IDictionary<string, ILocalizationSource> _sources;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocalizationManager(ILocalizationConfiguration configuration, IIocResolver iocResolver)
        {
            Logger = NullLogger.Instance;
            _configuration = configuration;
            _iocResolver = iocResolver;
            _sources = new Dictionary<string, ILocalizationSource>();
        }

        public void Initialize()
        {
            InitializeSources();
        }

        public IReadOnlyList<LanguageInfo> GetAllLanguages()
        {
            return _configuration.Languages.ToImmutableList();
        }

        private void InitializeSources()
        {
            if (!_configuration.IsEnabled)
            {
                Logger.Debug("Localization disabled.");
                return;
            }

            Logger.Debug(string.Format("Initializing {0} localization sources.", _configuration.Sources.Count));
            foreach (var source in _configuration.Sources)
            {
                if (_sources.ContainsKey(source.Name))
                {
                    throw new AbpException("There are more than one source with name: " + source.Name + "! Source name must be unique!");
                }

                _sources[source.Name] = source;
                source.Initialize(_configuration, _iocResolver);

                //Extending dictionaries
                if (source is IDictionaryBasedLocalizationSource)
                {
                    var dictionaryBasedSource = source as IDictionaryBasedLocalizationSource;
                    var extensions = _configuration.Sources.Extensions.Where(e => e.SourceName == source.Name).ToList();
                    foreach (var extension in extensions)
                    {
                        foreach (var dictionaryInfo in extension.DictionaryProvider.GetDictionaries(source.Name))
                        {
                            dictionaryBasedSource.Extend(dictionaryInfo.Dictionary);
                        }
                    }
                }

                Logger.Debug("Initialized localization source: " + source.Name);
            }
        }

        /// <summary>
        /// Gets a localization source with name.
        /// </summary>
        /// <param name="name">Unique name of the localization source</param>
        /// <returns>The localization source</returns>
        public ILocalizationSource GetSource(string name)
        {
            if (!_configuration.IsEnabled)
            {
                return NullLocalizationSource.Instance;
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            ILocalizationSource source;
            if (!_sources.TryGetValue(name, out source))
            {
                throw new AbpException("Can not find a source with name: " + name);
            }

            return source;
        }

        /// <summary>
        /// Gets all registered localization sources.
        /// </summary>
        /// <returns>List of sources</returns>
        public IReadOnlyList<ILocalizationSource> GetAllSources()
        {
            return _sources.Values.ToImmutableList();
        }

        public string GetString(string sourceName, string name)
        {
            return GetSource(sourceName).GetString(name);
        }

        public string GetString(string sourceName, string name, CultureInfo culture)
        {
            return GetSource(sourceName).GetString(name, culture);
        }

        private LanguageInfo GetCurrentLanguage()
        {
            if (_configuration.Languages.IsNullOrEmpty())
            {
                throw new AbpException("No language defined in this application. Define languages on startup configuration.");
            }

            var currentCultureName = Thread.CurrentThread.CurrentUICulture.Name;

            //Try to find exact match
            var currentLanguage = _configuration.Languages.FirstOrDefault(l => l.Name == currentCultureName);
            if (currentLanguage != null)
            {
                return currentLanguage;
            }

            //Try to find best match
            currentLanguage = _configuration.Languages.FirstOrDefault(l => currentCultureName.StartsWith(l.Name));
            if (currentLanguage != null)
            {
                return currentLanguage;
            }

            //Try to find default language
            currentLanguage = _configuration.Languages.FirstOrDefault(l => l.IsDefault);
            if (currentLanguage != null)
            {
                return currentLanguage;
            }

            //Get first one
            return _configuration.Languages[0];
        }
    }
}