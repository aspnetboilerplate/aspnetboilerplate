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

            LocalIocManager.Using<ISettingManager>(async settingManager =>
            {
                if (AbpSession.TenantId is int tenantId)
                {
                    await settingManager.ChangeSettingForTenantAsync(tenantId, AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled, isLockoutEnabledByDefault.ToString());
                }
                else
                {
                    await settingManager.ChangeSettingForApplicationAsync(AbpZeroSettingNames.UserManagement.UserLockOut.IsEnabled, isLockoutEnabledByDefault.ToString());
                }
            });

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
    }
}
