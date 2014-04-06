using System;
using System.Collections.Generic;
using System.Collections.Immutable;

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

        public void RegisterSource(ILocalizationSource source)
        {
            if (_sources.ContainsKey(source.Name))
            {
                throw new AbpException("There is already a source with name: " + source.Name);
            }

            _sources[source.Name] = source;
        }

        public ILocalizationSource GetSource(string name)
        {
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

        public IReadOnlyList<ILocalizationSource> GetAllSources()
        {
            return _sources.Values.ToImmutableList();
        }
    }
}