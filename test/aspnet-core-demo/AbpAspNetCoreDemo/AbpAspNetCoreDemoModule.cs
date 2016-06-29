using System.Reflection;
using Abp.AspNetCore;
using Abp.AspNetCore.Configuration;
using Abp.EntityFrameworkCore;
using Abp.Localization;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Json;
using Abp.Modules;
using AbpAspNetCoreDemo.Core;

namespace AbpAspNetCoreDemo
{
    [DependsOn(
        typeof(AbpAspNetCoreModule), 
        typeof(AbpAspNetCoreDemoCoreModule), 
        typeof(AbpEntityFrameworkCoreModule)
        )]
    public class AbpAspNetCoreDemoModule : AbpModule
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
                        "AbpAspNetCoreDemo.Localization.SourceFiles"
                    )
                )
            );

            Configuration.Modules.AbpAspNetCore()
                .CreateControllersForAppServices(
                    typeof(AbpAspNetCoreDemoCoreModule).Assembly,
                    useConventionalHttpVerbs: true,
                    moduleName: "appcore"
                );
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}