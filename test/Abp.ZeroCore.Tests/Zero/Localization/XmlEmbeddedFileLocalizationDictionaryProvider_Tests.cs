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
    public class XmlEmbeddedFileLocalizationDictionaryProvider_Tests : AbpIntegratedTestBase<MyCustomXmlLangModule>
    {
        [Fact]
        public void Test_Xml_Override()
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

    public class MyCustomXmlLangModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    AbpZeroConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(AbpZeroCommonModule).GetAssembly(), "Abp.Zero.Localization.Source"
                    )
                )
            );

            Configuration.Localization.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo(
                    AbpConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MyCustomXmlLangModule).GetAssembly(), "Abp.Zero.Localization.Sources.Extensions.Xml.Abp"
                    )
                )
            );

            Configuration.Localization.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo(
                    AbpZeroConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(MyCustomXmlLangModule).GetAssembly(), "Abp.Zero.Localization.Sources.Extensions.Xml.AbpZero"
                    )
                )
            );
        }
    }
}
