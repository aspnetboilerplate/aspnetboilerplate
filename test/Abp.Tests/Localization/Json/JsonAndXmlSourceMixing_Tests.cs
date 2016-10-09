using System.Globalization;
using System.Reflection;
using System.Threading;
using Abp.Localization;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Json;
using Abp.Localization.Dictionaries.Xml;
using Abp.Localization.Sources;
using Abp.Modules;
using Shouldly;
using Xunit;

namespace Abp.Tests.Localization.Json
{
    public class JsonAndXmlSourceMixing_Tests : TestBaseWithLocalIocManager
    {
        private readonly AbpBootstrapper _bootstrapper;

        public JsonAndXmlSourceMixing_Tests()
        {
            LocalIocManager.Register<ILanguageManager, LanguageManager>();
            LocalIocManager.Register<ILanguageProvider, DefaultLanguageProvider>();

            _bootstrapper = AbpBootstrapper.Create<MyLangModule>(LocalIocManager);
            _bootstrapper.Initialize();
        }

        [Fact]
        public void Test_Xml_Json()
        {
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en");

            var mananger = LocalIocManager.Resolve<LocalizationManager>();

            var source = mananger.GetSource("Lang");

            source.GetString("Apple").ShouldBe("Apple");
            source.GetString("Banana").ShouldBe("Banana");
            source.GetString("ThisIsATest").ShouldBe("This is a test.");
            source.GetString("HowAreYou").ShouldBe("How are you?");

            Thread.CurrentThread.CurrentUICulture = new CultureInfo("zh-CN");

            source.GetString("Apple").ShouldBe("苹果");
            source.GetString("Banana").ShouldBe("香蕉");
            source.GetString("ThisIsATest").ShouldBe("这是一个测试.");
            source.GetString("HowAreYou").ShouldBe("你好吗?");
        }
    }

    public class MyLangModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    "Lang",
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(),
                         "Abp.Tests.Localization.Json.XmlSources"
                        )
                    )
                );

            Configuration.Localization.Sources.Extensions.Add(
                new LocalizationSourceExtensionInfo(
                    "Lang",
                    new JsonEmbeddedFileLocalizationDictionaryProvider(
                        Assembly.GetExecutingAssembly(),
                         "Abp.Tests.Localization.Json.JsonSources"
                        )));

            
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
