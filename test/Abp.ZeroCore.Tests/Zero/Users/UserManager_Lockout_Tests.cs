using System.Threading.Tasks;
using Abp.Configuration;
using Abp.Dependency;
using Abp.Zero.Configuration;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace Abp.Zero.Users
{
    public class UserManager_Lockout_Tests : AbpZeroTestBase
    {
        private readonly UserManager _userManager;

        public UserManager_Lockout_Tests()
        {
            _userManager = Resolve<UserManager>();
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

            (await _userManager.GetLockoutEnabledAsync(user)).ShouldBe(isLockoutEnabledByDefault);
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

            (await _userManager.GetLockoutEnabledAsync(user)).ShouldBe(isLockoutEnabledExpected);
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
