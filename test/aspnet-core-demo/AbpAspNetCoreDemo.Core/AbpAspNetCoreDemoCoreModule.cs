using Abp.AutoMapper;
using Abp.Localization;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Json;
using Abp.Modules;
using Abp.Reflection.Extensions;

namespace AbpAspNetCoreDemo.Core;

[DependsOn(typeof(AbpAutoMapperModule))]
public class AbpAspNetCoreDemoCoreModule : AbpModule
{
    public override void PreInitialize()
    {
        Configuration.Auditing.IsEnabledForAnonymousUsers = true;

        Configuration.Localization.Languages.Add(new LanguageInfo("en", "English", isDefault: true));
        Configuration.Localization.Languages.Add(new LanguageInfo("tr", "Türkçe"));
        Configuration.Localization.Languages.Add(new LanguageInfo("pt-br", "Portuguese - Brazil"));
        Configuration.Localization.Languages.Add(new LanguageInfo("en-gb", "English - United Kingdom"));
        Configuration.Localization.Languages.Add(new LanguageInfo("ro", "Română"));

        Configuration.Localization.Sources.Add(
            new DictionaryBasedLocalizationSource("AbpAspNetCoreDemoModule",
                new JsonEmbeddedFileLocalizationDictionaryProvider(
                    typeof(AbpAspNetCoreDemoCoreModule).GetAssembly(),
                    "AbpAspNetCoreDemo.Core.Localization.SourceFiles"
                )
            )
        );
    }

    public override void Initialize()
    {
        IocManager.RegisterAssemblyByConvention(typeof(AbpAspNetCoreDemoCoreModule).GetAssembly());
    }
}