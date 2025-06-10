using System.Linq;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Configuration.Startup;
using Abp.Localization;
using Abp.Runtime.Session;
using Abp.Zero.Configuration;
using Abp.ZeroCore.SampleApp.Authorization;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace Abp.Zero.Users;

public class UserLogin_Tests : AbpZeroTestBase
{
    private readonly AppLogInManager _logInManager;

    public UserLogin_Tests()
    {
        UsingDbContext(UserLoginHelper.CreateTestUsers);
        _logInManager = LocalIocManager.Resolve<AppLogInManager>();
    }

    [Fact]
    public async Task Should_Login_With_Correct_Values_Without_MultiTenancy()
    {
        Resolve<IMultiTenancyConfig>().IsEnabled = false;
        AbpSession.TenantId = 1;

        var loginResult = await _logInManager.LoginAsync("user1", "123qwe");
        loginResult.Result.ShouldBe(AbpLoginResultType.Success);
        loginResult.User.Name.ShouldBe("User");
        loginResult.Identity.ShouldNotBe(null);

        UsingDbContext(context =>
        {
            context.UserLoginAttempts.Count().ShouldBe(1);
            context.UserLoginAttempts.FirstOrDefault(a =>
                a.TenantId == AbpSession.TenantId &&
                a.UserId == loginResult.User.Id &&
                a.UserNameOrEmailAddress == "user1" &&
                a.Result == AbpLoginResultType.Success
                ).ShouldNotBeNull();
        });
    }

    [Fact]
    public async Task Should_Not_Login_With_Invalid_UserName_Without_MultiTenancy()
    {
        Resolve<IMultiTenancyConfig>().IsEnabled = false;

        var loginResult = await _logInManager.LoginAsync("wrongUserName", "asdfgh");
        loginResult.Result.ShouldBe(AbpLoginResultType.InvalidUserNameOrEmailAddress);
        loginResult.User.ShouldBe(null);
        loginResult.Identity.ShouldBe(null);

        UsingDbContext(context =>
        {
            context.UserLoginAttempts.Count().ShouldBe(1);
            context.UserLoginAttempts.FirstOrDefault(a =>
                a.UserNameOrEmailAddress == "wrongUserName" &&
                a.Result == AbpLoginResultType.InvalidUserNameOrEmailAddress
                ).ShouldNotBeNull();
        });
    }

    [Fact]
    public async Task Should_Login_With_Correct_Values_With_MultiTenancy()
    {
        Resolve<IMultiTenancyConfig>().IsEnabled = true;
        AbpSession.TenantId = 1;

        var loginResult = await _logInManager.LoginAsync("user1", "123qwe", Tenant.DefaultTenantName);
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
        (await _logInManager.LoginAsync("user1", "123qwe", Tenant.DefaultTenantName)).Result.ShouldBe(AbpLoginResultType.Success);

        //Change configuration
        await Resolve<ISettingManager>().ChangeSettingForTenantAsync(AbpSession.GetTenantId(), AbpZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin, "true");

        //Email confirmation is enabled now
        (await _logInManager.LoginAsync("user1", "123qwe", Tenant.DefaultTenantName)).Result.ShouldBe(AbpLoginResultType.UserEmailIsNotConfirmed);
    }

    [Fact]
    public async Task Should_Login_TenancyOwner_With_Correct_Values()
    {
        Resolve<IMultiTenancyConfig>().IsEnabled = true;

        //Set session
        AbpSession.TenantId = 1;
        AbpSession.UserId = 1;

        var loginResult = await _logInManager.LoginAsync("userOwner", "123qwe", Tenant.DefaultTenantName);
        loginResult.Result.ShouldBe(AbpLoginResultType.Success);
        loginResult.User.Name.ShouldBe("Owner");
        loginResult.Identity.ShouldNotBe(null);
    }

    [Fact]
    public async Task Should_Save_LoginAttempt_With_FailReason()
    {
        Resolve<IMultiTenancyConfig>().IsEnabled = true;

        //Set session
        AbpSession.TenantId = 1;
        AbpSession.UserId = 1;

        var loginResult = await _logInManager.LoginAsync("forbidden-user", "123qwe", Tenant.DefaultTenantName);
        loginResult.Result.ShouldBe(AbpLoginResultType.FailedForOtherReason);

        var localizationContext = LocalIocManager.IocContainer.Resolve<ILocalizationContext>();
        loginResult.GetFailReason(localizationContext).ShouldBe("[Forbidden user]");

        UsingDbContext(context =>
        {
            var count = context.UserLoginAttempts.Count(e => e.FailReason == "[Forbidden user]");
            count.ShouldBeEquivalentTo(1);
        });
    }
}