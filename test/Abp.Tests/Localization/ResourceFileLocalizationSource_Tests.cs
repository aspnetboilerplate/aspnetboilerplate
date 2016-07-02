using System.Globalization;
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
            _resourceFileLocalizationSource.GetString("Hello", new CultureInfo("en")).ShouldBe("Hello!");

            //en-US and en-GB fallbacks to en
            _resourceFileLocalizationSource.GetString("Hello", new CultureInfo("en-US")).ShouldBe("Hello!");
            _resourceFileLocalizationSource.GetString("World", new CultureInfo("en-US")).ShouldBe("World!");
            _resourceFileLocalizationSource.GetString("Hello", new CultureInfo("en-GB")).ShouldBe("Hello!");

            //Defined in Turkish
            _resourceFileLocalizationSource.GetString("Hello", new CultureInfo("tr")).ShouldBe("Merhaba!");

            //tr-TR fallbacks to tr
            _resourceFileLocalizationSource.GetString("Hello", new CultureInfo("tr-TR")).ShouldBe("Merhaba!");

            //Undefined for Turkish, fallbacks to default language
            _resourceFileLocalizationSource.GetString("World", new CultureInfo("tr-TR")).ShouldBe("World!");

            //Undefined at all, fallback to given text
            _resourceFileLocalizationSource.GetString("Apple", new CultureInfo("en-US")).ShouldBe("[Apple]");
        }

        [Fact]
        public void Test_GetStringOrNull()
        {
            //Defined in English
            _resourceFileLocalizationSource.GetStringOrNull("Hello", new CultureInfo("en")).ShouldBe("Hello!");

            //en-US and en-GB fallbacks to en
            _resourceFileLocalizationSource.GetStringOrNull("Hello", new CultureInfo("en-US")).ShouldBe("Hello!");
            _resourceFileLocalizationSource.GetStringOrNull("World", new CultureInfo("en-US")).ShouldBe("World!");
            _resourceFileLocalizationSource.GetStringOrNull("Hello", new CultureInfo("en-GB")).ShouldBe("Hello!");

            //Defined in Turkish
            _resourceFileLocalizationSource.GetStringOrNull("Hello", new CultureInfo("tr")).ShouldBe("Merhaba!");

            //tr-TR fallbacks to tr
            _resourceFileLocalizationSource.GetStringOrNull("Hello", new CultureInfo("tr-TR")).ShouldBe("Merhaba!");

            //Undefined for Turkish, fallbacks to default language
            _resourceFileLocalizationSource.GetStringOrNull("World", new CultureInfo("tr-TR")).ShouldBe("World!");

            //Undefined at all, returns null
            _resourceFileLocalizationSource.GetStringOrNull("Apple", new CultureInfo("en-US")).ShouldBeNull();
        }
    }
}