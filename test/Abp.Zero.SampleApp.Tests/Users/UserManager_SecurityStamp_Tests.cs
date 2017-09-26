using System;
using System.Threading.Tasks;
using Abp.Threading;
using Abp.Zero.SampleApp.Users;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Users
{
    public class UserManager_SecurityStamp_Tests : SampleAppTestBase
    {
        private readonly UserManager _userManager;
        private readonly User _testUser;

        public UserManager_SecurityStamp_Tests()
        {
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
    }
}