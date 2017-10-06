using System.Threading.Tasks;
using Abp.TestBase;
using Abp.Web.Configuration;
using Shouldly;
using Xunit;

namespace Abp.Web.Common.Tests.Configuration
{
    public class AbpUserConfigurationBuilder_Tests : AbpIntegratedTestBase<AbpWebCommonTestModule>
    {
        private readonly AbpUserConfigurationBuilder _abpUserConfigurationBuilder;

        public AbpUserConfigurationBuilder_Tests()
        {
            _abpUserConfigurationBuilder = Resolve<AbpUserConfigurationBuilder>();
        }

        [Fact]
        public async Task AbpUserConfigurationBuilder_Should_Build_User_Configuration()
        {
            var userConfiguration = await _abpUserConfigurationBuilder.GetAll();
            userConfiguration.ShouldNotBe(null);
            
            userConfiguration.MultiTenancy.ShouldNotBe(null);
            userConfiguration.Session.ShouldNotBe(null);
            userConfiguration.Localization.ShouldNotBe(null);
            userConfiguration.Features.ShouldNotBe(null);
            userConfiguration.Auth.ShouldNotBe(null);
            userConfiguration.Nav.ShouldNotBe(null);
            userConfiguration.Setting.ShouldNotBe(null);
            userConfiguration.Clock.ShouldNotBe(null);
            userConfiguration.Timing.ShouldNotBe(null);
            userConfiguration.Security.ShouldNotBe(null);
        }
    }
}
