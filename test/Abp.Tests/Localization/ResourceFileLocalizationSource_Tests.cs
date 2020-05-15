using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Localization.Sources.Resource;
using Abp.Tests.Localization.TestResourceFiles;
using Shouldly;
using Xunit;

namespace Abp.Tests.Localization
{
    public class ResourceFileLocalizationSource_Tests
    {
        private readonly ResourceFileLocalizationSource _resourceFileLocalizationSource;

        public ResourceFileLocalizationSource_Tests()
        {
            _resourceFileLocalizationSource = new ResourceFileLocalizationSource("MyTestResource", MyTestResource.ResourceManager);
            _resourceFileLocalizationSource.Initialize(new LocalizationConfiguration
            {
                HumanizeTextIfNotFound = false,
                WrapGivenTextIfNotFound = true
            }, new IocManager());
        }

        [Fact]
        public void Test_GetString()
        {
            //Defined in English
            _resourceFileLocalizationSource.GetString("Hello", CultureInfo.GetCultureInfo("en")).ShouldBe("Hello!");

            //en-US and en-GB fallbacks to en
            _resourceFileLocalizationSource.GetString("Hello", CultureInfo.GetCultureInfo("en-US")).ShouldBe("Hello!");
            _resourceFileLocalizationSource.GetString("World", CultureInfo.GetCultureInfo("en-US")).ShouldBe("World!");
            _resourceFileLocalizationSource.GetString("Hello", CultureInfo.GetCultureInfo("en-GB")).ShouldBe("Hello!");

            //Defined in Turkish
            _resourceFileLocalizationSource.GetString("Hello", CultureInfo.GetCultureInfo("tr")).ShouldBe("Merhaba!");

            //tr-TR fallbacks to tr
            _resourceFileLocalizationSource.GetString("Hello", CultureInfo.GetCultureInfo("tr-TR")).ShouldBe("Merhaba!");

            //Undefined for Turkish, fallbacks to default language
            _resourceFileLocalizationSource.GetString("World", CultureInfo.GetCultureInfo("tr-TR")).ShouldBe("World!");

            //Undefined at all, fallback to given text
            _resourceFileLocalizationSource.GetString("Apple", CultureInfo.GetCultureInfo("en-US")).ShouldBe("[Apple]");
        }

        [Fact]
        public void Test_GetStringOrNull()
        {
            //Defined in English
            _resourceFileLocalizationSource.GetStringOrNull("Hello", CultureInfo.GetCultureInfo("en")).ShouldBe("Hello!");

            //en-US and en-GB fallbacks to en
            _resourceFileLocalizationSource.GetStringOrNull("Hello", CultureInfo.GetCultureInfo("en-US")).ShouldBe("Hello!");
            _resourceFileLocalizationSource.GetStringOrNull("World", CultureInfo.GetCultureInfo("en-US")).ShouldBe("World!");
            _resourceFileLocalizationSource.GetStringOrNull("Hello", CultureInfo.GetCultureInfo("en-GB")).ShouldBe("Hello!");

            //Defined in Turkish
            _resourceFileLocalizationSource.GetStringOrNull("Hello", CultureInfo.GetCultureInfo("tr")).ShouldBe("Merhaba!");

            //tr-TR fallbacks to tr
            _resourceFileLocalizationSource.GetStringOrNull("Hello", CultureInfo.GetCultureInfo("tr-TR")).ShouldBe("Merhaba!");

            //Undefined for Turkish, fallbacks to default language
            _resourceFileLocalizationSource.GetStringOrNull("World", CultureInfo.GetCultureInfo("tr-TR")).ShouldBe("World!");

            //Undefined at all, returns null
            _resourceFileLocalizationSource.GetStringOrNull("Apple", CultureInfo.GetCultureInfo("en-US")).ShouldBeNull();
        }

        [Fact]
        public void Test_GetStrings()
        {
            //Defined in English
            var enStrings = _resourceFileLocalizationSource.GetStrings(new List<string>{"Hello", "World"}, CultureInfo.GetCultureInfo("en"));
            enStrings.ShouldContain(x => x == "Hello!");
            enStrings.ShouldContain(x => x == "World!");

            //en-US and en-GB fallbacks to en
            var enUsStrings = _resourceFileLocalizationSource.GetStrings(new List<string>{"Hello", "World"}, CultureInfo.GetCultureInfo("en-US"));
            enUsStrings.ShouldContain(x => x == "Hello!");
            enUsStrings.ShouldContain(x => x == "World!");

            var enGBStrings = _resourceFileLocalizationSource.GetStrings(new List<string>{"Hello", "World"}, CultureInfo.GetCultureInfo("en-GB"));
            enGBStrings.ShouldContain(x => x == "Hello!");
            enGBStrings.ShouldContain(x => x == "World!");

            //Defined in Turkish
            _resourceFileLocalizationSource.GetStrings(new List<string>{"Hello"}, CultureInfo.GetCultureInfo("tr")).ShouldContain(x => x == "Merhaba!");

            //tr-TR fallbacks to tr
            _resourceFileLocalizationSource.GetStrings(new List<string>{"Hello"}, CultureInfo.GetCultureInfo("tr-TR")).ShouldContain(x => x == "Merhaba!");

            //Undefined for Turkish, fallbacks to default language
            _resourceFileLocalizationSource.GetStrings(new List<string>{"World"}, CultureInfo.GetCultureInfo("tr-TR")).ShouldContain(x => x == "World!");

            //Undefined at all, fallback to given text
            _resourceFileLocalizationSource.GetStrings(new List<string>{"Apple"}, CultureInfo.GetCultureInfo("en-US")).ShouldContain(x => x == "[Apple]");
        }

        [Fact]
        public void Test_GetStringsOrNull()
        {
            //Defined in English
            _resourceFileLocalizationSource.GetStringOrNull("Hello", CultureInfo.GetCultureInfo("en")).ShouldBe("Hello!");

            //en-US and en-GB fallbacks to en
            var enUsStrings = _resourceFileLocalizationSource.GetStringsOrNull(new List<string>{"Hello", "World"}, CultureInfo.GetCultureInfo("en-US"));
            enUsStrings.ShouldContain(x => x == "Hello!");
            enUsStrings.ShouldContain(x => x == "World!");

            _resourceFileLocalizationSource.GetStringsOrNull(new List<string>{"Hello"},  CultureInfo.GetCultureInfo("en-GB")).ShouldContain(x => x == "Hello!");

            //Defined in Turkish
            _resourceFileLocalizationSource.GetStringsOrNull(new List<string>{"Hello"}, CultureInfo.GetCultureInfo("tr")).ShouldContain(x => x == "Merhaba!");

            //tr-TR fallbacks to tr
            _resourceFileLocalizationSource.GetStringsOrNull(new List<string>{"Hello"},  CultureInfo.GetCultureInfo("tr-TR")).ShouldContain(x => x == "Merhaba!");

            //Undefined for Turkish, fallbacks to default language
            _resourceFileLocalizationSource.GetStringsOrNull(new List<string>{"World"}, CultureInfo.GetCultureInfo("tr-TR")).ShouldContain(x => x == "World!");

            //Undefined at all, returns null
            _resourceFileLocalizationSource.GetStringsOrNull(new List<string>{"Apple"}, CultureInfo.GetCultureInfo("en-US")).ShouldContain(x => x == null);
        }

        [Fact]
        public void Test_GetStringsOrNull_When_Name_Not_Exist()
        {
            var enStrings = _resourceFileLocalizationSource.GetStringsOrNull(new List<string>{"Hello", "World", "NotExist"}, CultureInfo.GetCultureInfo("en"));
            enStrings.ShouldContain(x => x == "Hello!");
            enStrings.ShouldContain(x => x == "World!");
            enStrings.ShouldContain(x => x == null);
            enStrings[2].ShouldBeNull(); //NotExist
        }

        [Fact(Skip = "Waiting for https://github.com/aspnetboilerplate/aspnetboilerplate/issues/1995")]
        public void Test_GetAllStrings()
        {
            var allStrings = _resourceFileLocalizationSource.GetAllStrings(CultureInfo.GetCultureInfo("en"));
            allStrings.Count.ShouldBe(2);
            allStrings.Any(s => s.Name == "Hello" && s.Value == "Hello!").ShouldBeTrue();
            allStrings.Any(s => s.Name == "World" && s.Value == "World!").ShouldBeTrue();
        }
    }
}
