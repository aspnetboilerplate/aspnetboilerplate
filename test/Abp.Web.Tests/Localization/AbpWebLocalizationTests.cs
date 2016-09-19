using System.Globalization;
using System.Reflection;
using Abp.Localization;
using Abp.TestBase;
using Abp.Web.Localization;
using Shouldly;
using Xunit;

namespace Abp.Web.Tests.Localization
{
    public class AbpWebLocalizationTests : AbpIntegratedTestBase<AbpWebModule>
    {
        private readonly ILocalizationManager _localizationManager;

        public AbpWebLocalizationTests()
        {
            _localizationManager = Resolve<ILocalizationManager>();
        }

        [Fact]
        public void Should_Get_Localized_Strings()
        {
            var names = Assembly.GetAssembly(typeof(AbpWebModule)).GetManifestResourceNames();

            var source = _localizationManager.GetSource(AbpWebConsts.LocalizaionSourceName);
            source.GetString("Yes", new CultureInfo("en-US")).ShouldBe("Yes");
            source.GetString("Yes", new CultureInfo("tr-TR")).ShouldBe("Evet");
        }
    }
}
