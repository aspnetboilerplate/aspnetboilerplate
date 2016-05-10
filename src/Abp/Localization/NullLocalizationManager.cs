using System.Collections.Generic;
using System.Threading;
using Abp.Localization.Sources;

namespace Abp.Localization
{
    public class NullLocalizationManager : ILocalizationManager
    {
        private readonly IReadOnlyList<LanguageInfo> _emptyLanguageArray = new LanguageInfo[0];

        private readonly IReadOnlyList<ILocalizationSource> _emptyLocalizationSourceArray = new ILocalizationSource[0];

        private NullLocalizationManager()
        {
        }

        /// <summary>
        ///     Singleton instance.
        /// </summary>
        public static NullLocalizationManager Instance { get; } = new NullLocalizationManager();

        public LanguageInfo CurrentLanguage
        {
            get
            {
                return new LanguageInfo(Thread.CurrentThread.CurrentUICulture.Name,
                    Thread.CurrentThread.CurrentUICulture.DisplayName);
            }
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