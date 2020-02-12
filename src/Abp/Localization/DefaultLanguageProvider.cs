using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Configuration.Startup;
using Abp.Dependency;

namespace Abp.Localization
{
    public class DefaultLanguageProvider : ILanguageProvider, ITransientDependency
    {
        private readonly ILocalizationConfiguration _configuration;

        public DefaultLanguageProvider(ILocalizationConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IReadOnlyList<LanguageInfo> GetLanguages()
        {
            return _configuration.Languages.ToImmutableList();
        }

        public IReadOnlyList<LanguageInfo> GetActiveLanguages()
        {
            return _configuration.Languages.Where(l => !l.IsDisabled).ToImmutableList();
        }
    }
}