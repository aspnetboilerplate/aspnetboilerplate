using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using Abp.Dependency;

namespace Abp.Localization
{
    public class LanguageManager : ILanguageManager, ITransientDependency
    {
        public LanguageInfo CurrentLanguage { get { return GetCurrentLanguage(); } }

        private readonly ILanguageProvider _languageProvider;

        public LanguageManager(ILanguageProvider languageProvider)
        {
            _languageProvider = languageProvider;
        }

        public IReadOnlyList<LanguageInfo> GetLanguages()
        {
            return _languageProvider.GetLanguages();
        }

        private LanguageInfo GetCurrentLanguage()
        {
            var languages = _languageProvider.GetLanguages();
            if (languages.Count <= 0)
            {
                throw new AbpException("No language defined in this application.");
            }

            var currentCultureName = CultureInfo.CurrentUICulture.Name;

            //Try to find exact match
            var currentLanguage = languages.FirstOrDefault(l => l.Name == currentCultureName);
            if (currentLanguage != null)
            {
                return currentLanguage;
            }

            //Try to find best match
            currentLanguage = languages.FirstOrDefault(l => currentCultureName.StartsWith(l.Name));
            if (currentLanguage != null)
            {
                return currentLanguage;
            }

            //Try to find default language
            currentLanguage = languages.FirstOrDefault(l => l.IsDefault);
            if (currentLanguage != null)
            {
                return currentLanguage;
            }

            //Get first one
            return languages[0];
        }
    }
}