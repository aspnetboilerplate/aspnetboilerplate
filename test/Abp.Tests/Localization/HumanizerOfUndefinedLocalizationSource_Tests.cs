using System.Globalization;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Localization.Sources.Resource;
using Abp.Tests.Localization.TestResourceFiles;
using Shouldly;
using Xunit;

namespace Abp.Tests.Localization
{
    public class HumanizerOfUndefinedLocalizationSource_Tests
    {
        private readonly ResourceFileLocalizationSource _resourceFileLocalizationSource;
        private LocalizationConfiguration _localizationConfiguration;

        public HumanizerOfUndefinedLocalizationSource_Tests()
        {
            _localizationConfiguration = new LocalizationConfiguration{ WrapGivenTextIfNotFound = false };
            _resourceFileLocalizationSource = new ResourceFileLocalizationSource("MyTestResource", MyTestResource.ResourceManager);
            _resourceFileLocalizationSource.Initialize(_localizationConfiguration, new IocManager());
        }

        [Fact]
        public void Undefined_Localization_Source_Should_Be_Humanized()
        {
            // Fallback to the same text as It's already in sentence case
            _resourceFileLocalizationSource
                .GetString("Lorem ipsum dolor sit amet", new CultureInfo("en-US"))
                .ShouldBe("Lorem ipsum dolor sit amet");

            // Text in PascalCase should be converted properly
            _resourceFileLocalizationSource
                .GetString("LoremIpsumDolorSitAmet", new CultureInfo("en-US"))
                .ShouldBe("Lorem ipsum dolor sit amet");

            // Text with mixed cases should be converted properly
            _resourceFileLocalizationSource
                .GetString("LoremIpsum dolor sit amet", new CultureInfo("en-US"))
                .ShouldBe("Lorem ipsum dolor sit amet");
        }
    }
}
