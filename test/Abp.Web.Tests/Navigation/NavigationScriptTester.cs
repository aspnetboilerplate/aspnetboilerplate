using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.Runtime.Remoting;
using Abp.Runtime.Session;
using Abp.TestBase.Runtime.Session;
using Abp.Tests.Application.Navigation;
using Abp.Web.Navigation;
using Shouldly;
using Xunit;

namespace Abp.Web.Tests.Navigation
{
    public class NavigationScript_Tests
    {
        [Fact]
        public async Task Should_Get_Script()
        {
            var testCase = new NavigationTestCase();
            var scriptManager = new NavigationScriptManager(testCase.UserNavigationManager)
            {
                AbpSession = new TestAbpSession(new MultiTenancyConfig(), new DataContextAmbientScopeProvider<SessionOverride>(new CallContextAmbientDataContext())) { UserId = 1 }
            };

            var script = await scriptManager.GetScriptAsync();
            script.ShouldNotBeNullOrEmpty();
        }
    }
}
