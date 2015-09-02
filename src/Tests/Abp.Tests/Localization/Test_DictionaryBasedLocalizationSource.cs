using System.Globalization;
using System.Linq;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Localization.Sources;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Tests.Localization
{
    public class Test_DictionaryBasedLocalizationSource
    {
        private readonly DictionaryBasedLocalizationSource _localizationSource;

        public Test_DictionaryBasedLocalizationSource()
        {
            var dictionaryProvider = Substitute.For<ILocalizationDictionaryProvider>();

            dictionaryProvider.GetDictionaries("Test").Returns(
                new[]
                {
                    new LocalizationDictionaryInfo(
                        new LocalizationDictionaryWithAddMethod(new CultureInfo("en"))
                        {
                            {"hello", "Hello"},
                            {"world", "World"},
                            {"fourtyTwo", "Fourty Two (42)"}
                        }, true), //Default language
                    new LocalizationDictionaryInfo(
                        new LocalizationDictionaryWithAddMethod(new CultureInfo("tr"))
                        {
                            {"hello", "Merhaba"},
                            {"world", "Dünya"}
                        }),
                    new LocalizationDictionaryInfo(
                        new LocalizationDictionaryWithAddMethod(new CultureInfo("tr-TR"))
                        {
                            {"world", "Yeryüzü"}
                        }),


                });

            _localizationSource = new DictionaryBasedLocalizationSource("Test", dictionaryProvider);
            _localizationSource.Initialize(new LocalizationConfiguration(), new IocManager());
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

        [Fact]
        public void Should_Extend_LocalizationSource_Overriding()
        {
            _localizationSource.Extend(
                new LocalizationDictionaryWithAddMethod(new CultureInfo("tr"))
                {
                    {"hello", "Selam"},
                });

            _localizationSource.GetString("hello", new CultureInfo("tr-TR")).ShouldBe("Selam");
        }

        [Fact]
        public void Should_Extend_LocalizationSource_With_New_Language()
        {
            _localizationSource.Extend(
                new LocalizationDictionaryWithAddMethod(new CultureInfo("fr"))
                {
                    {"hello", "Bonjour"},
                });

            _localizationSource.GetString("hello", new CultureInfo("fr")).ShouldBe("Bonjour");
            _localizationSource.GetString("world", new CultureInfo("fr")).ShouldBe("World"); //not localed into french
        }

        [Fact]
        public void Should_Return_Given_Text_If_Not_Found()
        {
            _localizationSource.GetString("An undefined text").ShouldBe("[An undefined text]");
        }
    }
}