using System.Linq;
using System.Reflection;
using Adorable.Localization.Dictionaries.Json;
using Shouldly;
using Xunit;

namespace Adorable.Tests.Localization.Json
{
    public class JsonEmbeddedFileLocalizationDictionaryProvider_Tests
    {
        private readonly JsonEmbeddedFileLocalizationDictionaryProvider _dictionaryProvider;

        public JsonEmbeddedFileLocalizationDictionaryProvider_Tests()
        {
            _dictionaryProvider = new JsonEmbeddedFileLocalizationDictionaryProvider(
                Assembly.GetExecutingAssembly(),
                "Adorable.Tests.Localization.Json.JsonSources"
                );

            _dictionaryProvider.Initialize("Lang");
        }

        [Fact]
        public void Should_Get_Dictionaries()
        {
            var dictionaries = _dictionaryProvider.Dictionaries.Values.ToList();

            dictionaries.Count.ShouldBe(2);

            var enDict = dictionaries.FirstOrDefault(d => d.CultureInfo.Name == "en");
            enDict.ShouldNotBe(null);
            enDict["Apple"].ShouldBe("Apple");
            enDict["Banana"].ShouldBe("Banana");

            var zhCNDict = dictionaries.FirstOrDefault(d => d.CultureInfo.Name == "zh-CN");
            zhCNDict.ShouldNotBe(null);
            zhCNDict["Apple"].ShouldBe("苹果");
            zhCNDict["Banana"].ShouldBe("香蕉");
        }
    }
}
