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
            _resourceFileLocalizationSource.Initialize(new LocalizationConfiguration(), new IocManager());
        }

        [Fact]
        public void Test_All()
        {
            _resourceFileLocalizationSource.GetString("Hello", new CultureInfo("en")).ShouldBe("Hello!");
            _resourceFileLocalizationSource.GetString("Hello", new CultureInfo("en-GB")).ShouldBe("Hello!");
            _resourceFileLocalizationSource.GetString("Hello", new CultureInfo("en-US")).ShouldBe("Hello!");
            _resourceFileLocalizationSource.GetString("World", new CultureInfo("en-US")).ShouldBe("World!");
            
            _resourceFileLocalizationSource.GetString("Hello", new CultureInfo("tr")).ShouldBe("Merhaba!");
            _resourceFileLocalizationSource.GetString("Hello", new CultureInfo("tr-TR")).ShouldBe("Merhaba!");

            //Undefined for Turkish, Fallback to default language
            _resourceFileLocalizationSource.GetString("World", new CultureInfo("tr-TR")).ShouldBe("World!");
            
            //Undefined at all, fallback to given text
            _resourceFileLocalizationSource.GetString("Apple", new CultureInfo("en-US")).ShouldBe("[Apple]");
        }
    }
}