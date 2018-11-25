using System;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Threading;
using Abp.Zero.Configuration;
using Abp.Zero.SampleApp.Authorization;
using Abp.Zero.SampleApp.Users;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Users
{
    public class UserManager_Lockout_Tests : SampleAppTestBase
    {
        private readonly UserManager _userManager;
        private readonly AppLogInManager _logInManager;
        private readonly User _testUser;

        public UserManager_Lockout_Tests()
        {
            _userManager = Resolve<UserManager>();
            _logInManager = Resolve<AppLogInManager>();

            _testUser = AsyncHelper.RunSync(() => CreateUser("TestUser"));

            Resolve<ISettingManager>()
                .ChangeSettingForApplicationAsync(
                    AbpZeroSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds,
                    "1"
                );
        }

        [Fact]
        public void Test_SupportsUserLockout()
        {
            _userManager.SupportsUserLockout.ShouldBeTrue();
        }

        [Fact]
        public async Task Test_Lockout_Full()
        {
            _userManager.InitializeLockoutSettings(_testUser.TenantId);

            for (int i = 0; i < _userManager.MaxFailedAccessAttemptsBeforeLockout; i++)
            {
                (await _userManager.IsLockedOutAsync(_testUser.Id)).ShouldBeFalse();
                await _userManager.AccessFailedAsync(_testUser.Id);
            }

            (await _userManager.IsLockedOutAsync(_testUser.Id)).ShouldBeTrue();

            await Task.Delay(TimeSpan.FromSeconds(1.5)); //Wait for unlock

            (await _userManager.IsLockedOutAsync(_testUser.Id)).ShouldBeFalse();
        }

        [Fact]
        public async Task Test_Login_Lockout()
        {
            (await _logInManager.LoginAsync("TestUser", "123qwe")).Result.ShouldBe(AbpLoginResultType.Success);

            for (int i = 0; i < _userManager.MaxFailedAccessAttemptsBeforeLockout - 1; i++)
            {
                (await _logInManager.LoginAsync("TestUser", "invalid-pass")).Result.ShouldBe(AbpLoginResultType.InvalidPassword);
            }

            (await _logInManager.LoginAsync("TestUser", "invalid-pass")).Result.ShouldBe(AbpLoginResultType.LockedOut);
            (await _userManager.IsLockedOutAsync(_testUser.Id)).ShouldBeTrue();

            await Task.Delay(TimeSpan.FromSeconds(1.5)); //Wait for unlock

            (await _userManager.GetAccessFailedCountAsync(_testUser.Id)).ShouldBe(0);
            (await _userManager.IsLockedOutAsync(_testUser.Id)).ShouldBeFalse();
            (await _logInManager.LoginAsync("TestUser", "invalid-pass")).Result.ShouldBe(AbpLoginResultType.InvalidPassword);

            (await _logInManager.LoginAsync("TestUser", "123qwe")).Result.ShouldBe(AbpLoginResultType.Success);
            (await _userManager.GetAccessFailedCountAsync(_testUser.Id)).ShouldBe(0);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Test_IsLockoutEnabled_On_User_Creation_Should_Use_Setting_By_Default(bool isLockoutEnabledByDefault)
        {
            // Arrange

            ChangeLockoutEnabledSetting(isLockoutEnabledByDefault);

            // Act

            var user = new User
            {
                TenantId = AbpSession.TenantId,
                UserName = "user1",
                Name = "John",
                Surname = "Doe",
                EmailAddress = "user1@aspnetboilerplate.com",
                IsEmailConfirmed = true,
                Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==", //123qwe
                // IsLockoutEnabled = isLockoutEnabled
            };

            await WithUnitOfWorkAsync(async () => await _userManager.CreateAsync(user));

            // Assert

            (await _userManager.GetLockoutEnabledAsync(user.Id)).ShouldBe(isLockoutEnabledByDefault);
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, false, true)]
        [InlineData(false, true, true)]
        [InlineData(false, false, false)]
        public async Task Test_IsLockoutEnabled_On_User_Creation_Should_Use_Stricter_Value_If_Set(bool isLockoutEnabledByDefault, bool isLockoutEnabled, bool isLockoutEnabledExpected)
        {
            // Arrange

            ChangeLockoutEnabledSetting(isLockoutEnabledByDefault);

            // Act

            var user = new User
            {
                TenantId = AbpSession.TenantId,
                UserName = "user1",
                Name = "John",
                Surname = "Doe",
                EmailAddress = "user1@aspnetboilerplate.com",
                IsEmailConfirmed = true,
                Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==", //123qwe
                IsLockoutEnabled = isLockoutEnabled
            };

            await WithUnitOfWorkAsync(async () => await _userManager.CreateAsync(user));

            // Assert

            (await _userManager.GetLockoutEnabledAsync(user.Id)).ShouldBe(isLockoutEnabledExpected);
        }

        #region Helpers

        private void ChangeLockoutEnabledSetting(bool isLockoutEnabledByDefault)
        {

            LocalIocManager.Using<ISettingManager>(settingManager =>
            {
                if (AbpSession.TenantId is int tenantId)
                {
                    settingManager.ChangeSettingForTenant(tenantId, AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled, isLockoutEnabledByDefault.ToString());
                }
                else
                {
                    settingManager.ChangeSettingForApplication(AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled, isLockoutEnabledByDefault.ToString());
                }
            });
        }

        #endregion
    }
}
