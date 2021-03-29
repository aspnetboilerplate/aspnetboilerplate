using System.Globalization;
using Abp.IdentityFramework;
using Abp.Localization;
using Microsoft.AspNet.Identity;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.IdentityFramework
{
    public class IdentityResultHelper_Tests : SampleAppTestBase
    {
        [Fact]
        public void Should_Localize_IdentityFramework_Messages()
        {
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-US");

            var localizationManager = Resolve<ILocalizationManager>();

            IdentityResult.Failed("Incorrect password.")
                .LocalizeErrors(localizationManager)
                .ShouldBe("Incorrect password.");

            IdentityResult.Failed("Passwords must be at least 6 characters.")
                .LocalizeErrors(localizationManager)
                .ShouldBe("Passwords must be at least 6 characters.");
        }
    }
}
