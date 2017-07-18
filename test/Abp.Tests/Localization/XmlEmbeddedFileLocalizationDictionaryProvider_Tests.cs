using System.Linq;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;
using Abp.Web;
using Shouldly;
using Xunit;

namespace Abp.Tests.Localization
{
    public class XmlEmbeddedFileLocalizationDictionaryProvider_Tests
    {
        private readonly XmlEmbeddedFileLocalizationDictionaryProvider _dictionaryProviderTest;
        private readonly XmlEmbeddedFileLocalizationDictionaryProvider _dictionaryProviderProd;

        public XmlEmbeddedFileLocalizationDictionaryProvider_Tests()
        {
            _dictionaryProviderTest = new XmlEmbeddedFileLocalizationDictionaryProvider(
                typeof(XmlEmbeddedFileLocalizationDictionaryProvider_Tests).GetAssembly(),
                "Abp.Tests.Localization.TestXmlFiles"
                );

            _dictionaryProviderTest.Initialize("Test");

            _dictionaryProviderProd = new XmlEmbeddedFileLocalizationDictionaryProvider(
                typeof(AbpWebCommonModule).GetAssembly(),
                "Abp.Web.Localization.AbpWebXmlSource"
            );

            _dictionaryProviderProd.Initialize("AbpWeb");
        }

        [Fact]
        public void Should_Get_Dictionaries_Test()
        {
            var dictionaries = _dictionaryProviderTest.Dictionaries.Values.ToList();
            
            dictionaries.Count.ShouldBe(2);

            var enDict = dictionaries.FirstOrDefault(d => d.CultureInfo.Name == "en");
            enDict.ShouldNotBe(null);
            enDict.ShouldBe(_dictionaryProviderTest.DefaultDictionary);
            enDict["hello"].ShouldBe("Hello");
            
            var trDict = dictionaries.FirstOrDefault(d => d.CultureInfo.Name == "tr");
            trDict.ShouldNotBe(null);
            trDict["hello"].ShouldBe("Merhaba");
        }

        [Fact]
        public void Should_Get_Dictionaries_Prod()
        {
            var dictionaries = _dictionaryProviderProd.Dictionaries.Values.ToList();

            dictionaries.Count().ShouldBeGreaterThan(1);

            var enDict = dictionaries.FirstOrDefault(d => d.CultureInfo.Name == "en");
            enDict.ShouldNotBe(null);

            var trDict = dictionaries.FirstOrDefault(d => d.CultureInfo.Name == "tr");
            trDict.ShouldNotBe(null);
        }
    }
}
