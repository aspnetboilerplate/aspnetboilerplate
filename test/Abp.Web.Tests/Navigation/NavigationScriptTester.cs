using System.Threading.Tasks;
using Abp.Configuration.Startup;
using Abp.MultiTenancy;
using Abp.Runtime.Remoting;
using Abp.Runtime.Session;
using Abp.TestBase.Runtime.Session;
using Abp.Tests.Application.Navigation;
using Abp.Web.Navigation;
using NSubstitute;
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
                AbpSession = CreateTestAbpSession()
            };

            var script = await scriptManager.GetScriptAsync();
            script.ShouldNotBeNullOrEmpty();
        }

        private static TestAbpSession CreateTestAbpSession()
        {
            return new TestAbpSession(
                new MultiTenancyConfig { IsEnabled = true },
                new DataContextAmbientScopeProvider<SessionOverride>(
                    new AsyncLocalAmbientDataContext()
                ),
                Substitute.For<ITenantResolver>()
            );
        }
    }
}
