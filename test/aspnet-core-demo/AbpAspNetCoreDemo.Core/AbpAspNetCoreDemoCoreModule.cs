using System.Reflection;
using Abp.AutoMapper;
using Abp.Localization;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Json;
using Abp.Modules;

namespace AbpAspNetCoreDemo.Core
{
    [DependsOn(typeof(AbpAutoMapperModule))]
    public class AbpAspNetCoreDemoCoreModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Auditing.IsEnabledForAnonymousUsers = true;

            Configuration.Localization.Languages.Add(new LanguageInfo("en", "English", isDefault: true));
            Configuration.Localization.Languages.Add(new LanguageInfo("tr", "Türkçe"));

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource("AbpAspNetCoreDemoModule",
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(),
                        "AbpAspNetCoreDemo.Core.Localization.SourceFiles"
                    )
                )
            );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}