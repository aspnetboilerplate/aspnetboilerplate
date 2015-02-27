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
            
            dictionaries.FirstOrDefault(d => d.Dictionary.CultureInfo.Name == "tr").ShouldNotBe(null);
            dictionaries.FirstOrDefault(d => d.Dictionary.CultureInfo.Name == "en").ShouldNotBe(null);
            
            dictionaries.Single(d => d.Dictionary.CultureInfo.Name == "en").IsDefault.ShouldBe(true);
        }
    }
}
