using System.Collections.Generic;
using Abp.Exceptions;

namespace Abp.Localization
{
    public class LocalizationManager : ILocalizationManager
    {
        private readonly IDictionary<string, ILocalizationSource> _sources;

        public LocalizationManager()
        {
            _sources = new Dictionary<string, ILocalizationSource>();
        }

        public void RegisterSource(ILocalizationSource source)
        {
            if (_sources.ContainsKey(source.SourceName))
            {
                throw new AbpException("There is already a localization source with name: " + source.SourceName);
            }

            _sources[source.SourceName] = source;
        }

        public ILocalizationSource GetSource(string sourceName)
        {
            ILocalizationSource source;
            if (!_sources.TryGetValue(sourceName, out source))
            {
                throw new AbpException("Can not find a localization source for name: " + sourceName);
            }

            return source;
        }
    }
}