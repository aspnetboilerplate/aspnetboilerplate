using System.Collections.Generic;
using System.Globalization;
using Abp.Extensions;
using Abp.FluentValidation.Configuration;
using Abp.Localization;
using Abp.Localization.Sources;
using LanguageManager = FluentValidation.Resources.LanguageManager;

namespace Abp.FluentValidation
{
    public class AbpFluentValidationLanguageManager : LanguageManager
    {
        public AbpFluentValidationLanguageManager(
            ILocalizationManager localizationManager,
            ILanguageManager languageManager,
            IAbpFluentValidationConfiguration configuration)
        {
            if (!configuration.LocalizationSourceName.IsNullOrEmpty())
            {
                var source = localizationManager.GetSource(configuration.LocalizationSourceName);
                var languages = languageManager.GetLanguages();

                AddAllTranslations(source, languages);
            }
        }

        private void AddAllTranslations(ILocalizationSource source, IReadOnlyList<LanguageInfo> languages)
        {
            foreach (var language in languages)
            {
                var culture = new CultureInfo(language.Name);
                var translations = source.GetAllStrings(culture, false);
                AddTranslations(language.Name, translations);
            }
        }

        private void AddTranslations(string language, IReadOnlyList<LocalizedString> translations)
        {
            foreach (var translation in translations)
            {
                AddTranslation(language, translation.Name, translation.Value);
            }
        }
    }
}
