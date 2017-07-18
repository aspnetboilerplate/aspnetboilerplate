using Abp.Localization;
using Abp.TestBase;
using Shouldly;
using Xunit;

namespace Abp.Web.Common.Tests.Web.Localization
{
    public class AbpWebCommonLocalization_Tests : AbpIntegratedTestBase<AbpWebCommonTestModule>
    {
        private readonly ILocalizationManager _localizationManager;

        public AbpWebCommonLocalization_Tests()
        {
            _localizationManager = Resolve<ILocalizationManager>();
        }

        [Fact]
        public void Should_Localize_AbpWebCommon_Texts()
        {
            using (CultureInfoHelper.Use("en"))
            {
                _localizationManager
                    .GetSource(AbpWebConsts.LocalizaionSourceName)
                    .GetString("ValidationError")
                    .ShouldBe("Your request is not valid!");
            }
        }
    }
}
