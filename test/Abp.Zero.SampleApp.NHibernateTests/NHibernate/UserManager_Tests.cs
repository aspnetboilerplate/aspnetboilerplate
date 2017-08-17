using System.Threading.Tasks;
using Abp.Zero.SampleApp.Users;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.NHibernate
{
    public class UserManager_Tests : NHibernateTestBase
    {
        private readonly UserManager _userManager;

        public UserManager_Tests()
        {
            _userManager = Resolve<UserManager>();
        }

        [Fact]
        public async Task Should_Find_User_By_Name()
        {
            var admin = await _userManager.FindByNameAsync(User.AdminUserName);
            admin.ShouldNotBeNull();
            admin.UserName.ShouldBe(User.AdminUserName);
        }
    }
}