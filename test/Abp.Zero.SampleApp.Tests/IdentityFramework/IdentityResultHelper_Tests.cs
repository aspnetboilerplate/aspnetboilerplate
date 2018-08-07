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

            IdentityResultExtensions
                .LocalizeErrors(IdentityResult.Failed("Incorrect password."), localizationManager)
                .ShouldBe("Incorrect password.");

            IdentityResultExtensions
                .LocalizeErrors(IdentityResult.Failed("Passwords must be at least 6 characters."), localizationManager)
                .ShouldBe("Passwords must be at least 6 characters.");
        }
    }
}
