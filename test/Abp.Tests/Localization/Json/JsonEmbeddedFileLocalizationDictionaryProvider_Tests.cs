using System.Linq;
using System.Reflection;
using Abp.Localization.Dictionaries.Json;
using Abp.Reflection.Extensions;
using Shouldly;
using Xunit;

namespace Abp.Tests.Localization.Json
{
    public class JsonEmbeddedFileLocalizationDictionaryProvider_Tests
    {
        private readonly JsonEmbeddedFileLocalizationDictionaryProvider _dictionaryProvider;

        public JsonEmbeddedFileLocalizationDictionaryProvider_Tests()
        {
            _dictionaryProvider = new JsonEmbeddedFileLocalizationDictionaryProvider(
                typeof(JsonEmbeddedFileLocalizationDictionaryProvider_Tests).GetAssembly(),
                "Abp.Tests.Localization.Json.JsonSources"
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

            var zhHansDict = dictionaries.FirstOrDefault(d => d.CultureInfo.Name == "zh-Hans");
            zhHansDict.ShouldNotBe(null);
            zhHansDict["Apple"].ShouldBe("苹果");
            zhHansDict["Banana"].ShouldBe("香蕉");
        }
    }
}
