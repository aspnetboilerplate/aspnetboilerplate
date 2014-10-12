using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Localization.Sources;

namespace Abp.Localization
{
    internal class LocalizationManager : ILocalizationManager
    {
        /// <summary>
        /// Gets current language for the application.
        /// </summary>
        public LanguageInfo CurrentLanguage { get { return GetCurrentLanguage(); } }

        private readonly IAbpStartupConfiguration _configuration;
        private readonly IDictionary<string, ILocalizationSource> _sources;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocalizationManager(IAbpStartupConfiguration configuration)
        {
            _configuration = configuration;
            _sources = new Dictionary<string, ILocalizationSource>();
        }

        public IReadOnlyList<LanguageInfo> GetAllLanguages()
        {
            return _configuration.Localization.Languages.ToImmutableList();
        }

        /// <summary>
        /// Registers new localization source.
        /// </summary>
        /// <param name="source">Localization source</param>
        public void RegisterSource(ILocalizationSource source)
        {
            if (!_configuration.Localization.IsEnabled)
            {
                return;
            }

            if (_sources.ContainsKey(source.Name))
            {
                throw new AbpException("There is already a source with name: " + source.Name);
            }

            _sources[source.Name] = source;
            source.Initialize();
        }

        /// <summary>
        /// Gets a localization source with name.
        /// </summary>
        /// <param name="name">Unique name of the localization source</param>
        /// <returns>The localization source</returns>
        public ILocalizationSource GetSource(string name)
        {
            if (!_configuration.Localization.IsEnabled)
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

        private LanguageInfo GetCurrentLanguage()
        {
            if (_configuration.Localization.Languages.IsNullOrEmpty())
            {
                throw new AbpException("No language defined in this application. Define languages on startup configuration.");
            }

            var currentCultureName = Thread.CurrentThread.CurrentUICulture.Name;

            //Try to find exact match
            var currentLanguage = _configuration.Localization.Languages.FirstOrDefault(l => l.Code == currentCultureName);
            if (currentLanguage != null)
            {
                return currentLanguage;
            }

            //Try to find best match
            currentLanguage = _configuration.Localization.Languages.FirstOrDefault(l => currentCultureName.StartsWith(l.Code));
            if (currentLanguage != null)
            {
                return currentLanguage;
            }

            //Try to find default language
            currentLanguage = _configuration.Localization.Languages.FirstOrDefault(l => l.IsDefault);
            if (currentLanguage != null)
            {
                return currentLanguage;
            }

            //Get first one
            return _configuration.Localization.Languages[0];
        }
    }
}