using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Runtime.Session;
using Abp.Zero.Configuration;
using Abp.Zero.SampleApp.MultiTenancy;
using Abp.Zero.SampleApp.Users;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Users
{
    public class UserLogin_Tests : SampleAppTestBase
    {
        private readonly UserManager _userManager;

        public UserLogin_Tests()
        {
            UsingDbContext(UserLoginHelper.CreateTestUsers);
            _userManager = LocalIocManager.Resolve<UserManager>();
        }

        [Fact]
        public async Task Should_Login_With_Correct_Values_Without_MultiTenancy()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = false;
            AbpSession.TenantId = 1; //TODO: We should not need to set this and implement AbpSession instead of TestSession.

            var loginResult = await _userManager.LoginAsync("user1", "123qwe");
            loginResult.Result.ShouldBe(AbpLoginResultType.Success);
            loginResult.User.Name.ShouldBe("User");
            loginResult.Identity.ShouldNotBe(null);
        }

        [Fact]
        public async Task Should_Not_Login_With_Invalid_UserName_Without_MultiTenancy()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = false;
            //AbpSession.TenantId = 1; //TODO: We should not need to set this and implement AbpSession instead of TestSession.

            var loginResult = await _userManager.LoginAsync("wrongUserName", "asdfgh");
            loginResult.Result.ShouldBe(AbpLoginResultType.InvalidUserNameOrEmailAddress);
            loginResult.User.ShouldBe(null);
            loginResult.Identity.ShouldBe(null);
        }

        [Fact]
        public async Task Should_Login_With_Correct_Values_With_MultiTenancy()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            AbpSession.TenantId = 1;

            var loginResult = await _userManager.LoginAsync("user1", "123qwe", Tenant.DefaultTenantName);
            loginResult.Result.ShouldBe(AbpLoginResultType.Success);
            loginResult.User.Name.ShouldBe("User");
            loginResult.Identity.ShouldNotBe(null);
        }

        [Fact]
        public async Task Should_Not_Login_If_Email_Confirmation_Is_Enabled_And_User_Has_Not_Confirmed()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            //Set session
            AbpSession.TenantId = 1;
            AbpSession.UserId = 1;

            //Email confirmation is disabled as default
            (await _userManager.LoginAsync("user1", "123qwe", Tenant.DefaultTenantName)).Result.ShouldBe(AbpLoginResultType.Success);

            //Change configuration
            await Resolve<ISettingManager>().ChangeSettingForTenantAsync(AbpSession.GetTenantId(), AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin, "true");

            //Email confirmation is enabled now
            (await _userManager.LoginAsync("user1", "123qwe", Tenant.DefaultTenantName)).Result.ShouldBe(AbpLoginResultType.UserEmailIsNotConfirmed);
        }

        [Fact]
        public async Task Should_Login_TenancyOwner_With_Correct_Values()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            var loginResult = await _userManager.LoginAsync("userOwner", "123qwe");
            loginResult.Result.ShouldBe(AbpLoginResultType.Success);
            loginResult.User.Name.ShouldBe("Owner");
            loginResult.Identity.ShouldNotBe(null);
        }
    }
}
