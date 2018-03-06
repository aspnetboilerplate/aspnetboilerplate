using Abp.Localization;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Json;
using Abp.Localization.Dictionaries.Xml;
using Abp.Localization.Sources;
using Abp.Modules;
using Abp.Reflection.Extensions;
using Abp.TestBase;
using Shouldly;
using Xunit;

namespace Abp.Zero.Localization
{
    public class JsonEmbeddedFileLocalizationDictionaryProvider_Tests : AbpIntegratedTestBase<MyCustomJsonLangModule>
    {
        [Fact]
        public void Test_Json_Override()
        {
            var mananger = LocalIocManager.Resolve<ILocalizationManager>();

            using (CultureInfoHelper.Use("en"))
            {
                var abpSource = mananger.GetSource(AbpConsts.LocalizationSourceName);
                abpSource.GetString("TimeZone").ShouldBe("Time-zone");

                var abpZeroSource = mananger.GetSource(AbpZeroConsts.LocalizationSourceName);
                abpZeroSource.GetString("Email").ShouldBe("Email address");
            }
        }
    }

    public class MyCustomJsonLangModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.Sources.Clear();

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    AbpConsts.LocalizationSourceName,
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MyCustomJsonLangModule).GetAssembly(),
                        "Abp.Zero.Localization.Sources.Base.Abp"
                    )
                )
            );

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    AbpZeroConsts.LocalizationSourceName,
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MyCustomJsonLangModule).GetAssembly(),
                        "Abp.Zero.Localization.Sources.Base.AbpZero"
                    )
                )
            );

            Configuration.Localization.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo(
                    AbpConsts.LocalizationSourceName,
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MyCustomJsonLangModule).GetAssembly(), "Abp.Zero.Localization.Sources.Extensions.Json.Abp"
                    )
                )
            );

            Configuration.Localization.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo(
                    AbpZeroConsts.LocalizationSourceName,
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MyCustomJsonLangModule).GetAssembly(), "Abp.Zero.Localization.Sources.Extensions.Json.AbpZero"
                    )
                )
            );
        }
    }
}
