using System.Linq;
using System.Reflection;
using Abp.Localization.Sources.Xml;
using Shouldly;
using Xunit;

namespace Abp.Tests.Localization
{
    public class XmlEmbeddedFileLocalizationDictionaryProvider_Tests
    {
        private readonly XmlEmbeddedFileLocalizationDictionaryProvider _dictionaryProvider;

        public XmlEmbeddedFileLocalizationDictionaryProvider_Tests()
        {
            _dictionaryProvider = new XmlEmbeddedFileLocalizationDictionaryProvider(
                Assembly.GetExecutingAssembly(),
                "Abp.Tests.Localization.TestXmlFiles"
                );
        }

        [Fact]
        public void Should_Get_Dictionaries()
        {
            var dictionaries = _dictionaryProvider.GetDictionaries("Test").ToList();
            
            dictionaries.Count.ShouldBe(2);

            var enDict = dictionaries.FirstOrDefault(d => d.Dictionary.CultureInfo.Name == "en");
            enDict.ShouldNotBe(null);
            enDict.IsDefault.ShouldBe(true);
            enDict.Dictionary["hello"].ShouldBe("Hello");
            
            var trDict = dictionaries.FirstOrDefault(d => d.Dictionary.CultureInfo.Name == "tr");
            trDict.ShouldNotBe(null);
            trDict.Dictionary["hello"].ShouldBe("Merhaba");            
        }
    }
}
