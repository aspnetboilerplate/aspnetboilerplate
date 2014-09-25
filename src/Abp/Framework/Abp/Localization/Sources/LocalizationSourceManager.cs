using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Startup.Configuration;

namespace Abp.Localization.Sources
{
    /// <summary>
    /// This class is used to manage localization sources by implementing <see cref="ILocalizationSourceManager"/>. See <see cref="ILocalizationSource"/>.
    /// </summary>
    public class LocalizationSourceManager : ILocalizationSourceManager
    {
        private readonly IDictionary<string, ILocalizationSource> _sources;

        /// <summary>
        /// Constructor.
        /// </summary>
        public LocalizationSourceManager()
        {
            _sources = new Dictionary<string, ILocalizationSource>();
        }

        /// <summary>
        /// Registers new localization source.
        /// </summary>
        /// <param name="source">Localization source</param>
        public void RegisterSource(ILocalizationSource source)
        {
            if (!AbpConfiguration.Instance.Localization.IsEnabled)
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
            if (!AbpConfiguration.Instance.Localization.IsEnabled)
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
    }
}