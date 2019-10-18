using System.Globalization;
using System.Threading;
using Abp.Localization;
using Shouldly;
using Xunit;

namespace Abp.Zero.Localization
{
    public class SimpleLocalization_Tests : AbpZeroTestBase
    {
        [Theory]
        [InlineData("en")]
        [InlineData("en-US")]
        [InlineData("en-GB")]
        public void Test1(string cultureName)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);

            Resolve<ILocalizationManager>().GetString(AbpZeroConsts.LocalizationSourceName, "Identity.UserNotInRole")
            .ShouldBe("User is not in role '{0}'.");
        }
    }
}
