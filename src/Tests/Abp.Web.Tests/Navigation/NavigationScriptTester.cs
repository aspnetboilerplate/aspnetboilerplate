using System.Threading.Tasks;
using Adorable.Tests.Application.Navigation;
using Adorable.Tests.Configuration;
using Adorable.Web.Navigation;
using Shouldly;
using Xunit;

namespace Adorable.Web.Tests.Navigation
{
    public class NavigationScript_Tests
    {
        [Fact]
        public async Task Should_Get_Script()
        {
            var testCase = new NavigationTestCase();
            var scriptManager = new NavigationScriptManager(testCase.UserNavigationManager)
            {
                AbpSession = new MyChangableSession { UserId = 1 }
            };

            var script = await scriptManager.GetScriptAsync();
            script.ShouldNotBeNullOrEmpty();
        }
    }
}
