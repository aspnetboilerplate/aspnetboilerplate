using System;
using System.Threading.Tasks;
using Abp.Threading;
using Abp.Zero.SampleApp.Users;
using Abp.Zero.SampleApp.Users.Dto;
using Microsoft.AspNet.Identity;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Users
{
    public class UserManager_SecurityStamp_Tests : SampleAppTestBase
    {
        private readonly UserManager _userManager;
        private readonly User _testUser;
        private readonly IUserAppService _userAppService;

        public UserManager_SecurityStamp_Tests()
        {
            _userAppService = Resolve<IUserAppService>();
            _userManager = Resolve<UserManager>();

            _userManager.DefaultAccountLockoutTimeSpan = TimeSpan.FromSeconds(0.5);

            _testUser = AsyncHelper.RunSync(() => CreateUser("TestUser"));
        }

        [Fact]
        public void Test_SecurityStamp()
        {
            _userManager.SupportsUserSecurityStamp.ShouldBeTrue();
        }

        [Fact]
        public async Task Test_Set_Get()
        {
            var oldStamp = await _userManager.GetSecurityStampAsync(_testUser.Id);

            await _userManager.UpdateSecurityStampAsync(_testUser.Id);

            (await _userManager.GetSecurityStampAsync(_testUser.Id)).ShouldNotBe(oldStamp);
        }
         [Fact]
        public async Task Should_Change_Security_Stamp()
        {
            _userAppService.CreateUser(
                new CreateUserInput
                {
                    EmailAddress = "security@stamp.com",
                    Name = "Security",
                    Surname = "Stamp",
                    UserName = "SecurityStamp"
                });

            var firstUser = await _userManager.FindByNameAsync("SecurityStamp");
            var firstSecurityStamp = firstUser.SecurityStamp;

            //change password
            await _userManager.ChangePasswordAsync(firstUser, "asd123");

            User passwordChangedUser = await _userManager.FindByIdAsync(firstUser.Id);
            var passwordChangedUserSecurityStamp = passwordChangedUser.SecurityStamp;
            passwordChangedUserSecurityStamp.ShouldNotBe(firstSecurityStamp);
        }
    }
}