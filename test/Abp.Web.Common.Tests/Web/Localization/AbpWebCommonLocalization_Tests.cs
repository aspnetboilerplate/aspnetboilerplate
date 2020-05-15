using System.Collections.Generic;
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
        public void Should_Localize_AbpWebCommon_Text()
        {
            using (CultureInfoHelper.Use("en"))
            {
                _localizationManager
                    .GetSource(AbpWebConsts.LocalizaionSourceName)
                    .GetString("ValidationError")
                    .ShouldBe("Your request is not valid!");
            }
        }

        [Fact]
        public void Should_Localize_AbpWebCommon_Texts()
        {
            using (CultureInfoHelper.Use("en"))
            {
                var texts = _localizationManager
                    .GetSource(AbpWebConsts.LocalizaionSourceName)
                    .GetStrings(new List<string> {"ValidationError", "InternalServerError"});

                texts.ShouldContain(x => x == "Your request is not valid!");
                texts.ShouldContain(x => x == "An internal error occurred during your request!");

            }
        }
    }
}
