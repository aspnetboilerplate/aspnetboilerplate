using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Abp.Localization.Sources;

namespace Abp.Localization
{
    public class NullLocalizationManager : ILocalizationManager
    {
        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static NullLocalizationManager Instance { get; } = new NullLocalizationManager();

        public LanguageInfo CurrentLanguage { get { return new LanguageInfo(CultureInfo.CurrentUICulture.Name, CultureInfo.CurrentUICulture.DisplayName); } }

        private readonly IReadOnlyList<LanguageInfo> _emptyLanguageArray = new LanguageInfo[0];

        private readonly IReadOnlyList<ILocalizationSource> _emptyLocalizationSourceArray = new ILocalizationSource[0];

        private NullLocalizationManager()
        {

        }

        public IReadOnlyList<LanguageInfo> GetAllLanguages()
        {
            return _emptyLanguageArray;
        }

        public ILocalizationSource GetSource(string name)
        {
            return NullLocalizationSource.Instance;
        }

        public IReadOnlyList<ILocalizationSource> GetAllSources()
        {
            return _emptyLocalizationSourceArray;
        }
    }
}
