using System.Globalization;
using System.Linq;
using Abp.Localization.Sources;
using NUnit.Framework;
using Xunit;

namespace Abp.Tests.Localization
{
    public class Test_DictionaryBasedLocalizationSource
    {
        private readonly DictionaryBasedLocalizationSource _localizationSource;

        public Test_DictionaryBasedLocalizationSource()
        {
            _localizationSource = new DictionaryBasedLocalizationSource("Test");

            _localizationSource.AddDictionary(new LocalizationDictionaryWithAddMethod(new CultureInfo("en"))
                                              {
                                                  {"hello", "Hello"},
                                                  {"world", "World"},
                                                  {"fourtyTwo", "Fourty Two (42)"}
                                              }, true);

            _localizationSource.AddDictionary(new LocalizationDictionaryWithAddMethod(new CultureInfo("tr"))
                                              {
                                                  {"hello", "Merhaba"},
                                                  {"world", "Dünya"}
                                              });

            _localizationSource.AddDictionary(new LocalizationDictionaryWithAddMethod(new CultureInfo("tr-TR"))
                                              {
                                                  {"world", "Yeryüzü"}
                                              });
        }

        [Fact]
        public void Should_Get_Correct_String_On_Exact_Culture()
        {
            Assert.Equal("Yeryüzü", _localizationSource.GetString("world", new CultureInfo("tr-TR")));
        }

        [Fact]
        public void Should_Get_Most_Close_String_On_Base_Culture()
        {
            Assert.Equal("Merhaba", _localizationSource.GetString("hello", new CultureInfo("tr-TR")));
        }

        [Fact]
        public void Should_Get_Default_If_Not_Exists_On_Given_Culture()
        {
            Assert.Equal("Fourty Two (42)", _localizationSource.GetString("fourtyTwo", new CultureInfo("tr")));
            Assert.Equal("Fourty Two (42)", _localizationSource.GetString("fourtyTwo", new CultureInfo("tr-TR")));
        }

        [Fact]
        public void Should_Get_All_Strings()
        {
            var localizedStrings = _localizationSource.GetAllStrings(new CultureInfo("tr-TR")).OrderBy(ls => ls.Name).ToList();
            Assert.Equal(3, localizedStrings.Count);
            Assert.Equal("Fourty Two (42)", localizedStrings[0].Value);
            Assert.Equal("Merhaba", localizedStrings[1].Value);
            Assert.Equal("Yeryüzü", localizedStrings[2].Value);
        }
    }
}