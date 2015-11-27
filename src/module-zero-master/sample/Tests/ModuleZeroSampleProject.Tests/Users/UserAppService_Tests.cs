using System.Linq;
using ModuleZeroSampleProject.Users;
using Shouldly;
using Xunit;

namespace ModuleZeroSampleProject.Tests.Users
{
    public class UserAppService_Tests : SampleProjectTestBase
    {
        private readonly IUserAppService _userAppService;

        public UserAppService_Tests()
        {
            _userAppService = LocalIocManager.Resolve<IUserAppService>();
        }

        [Fact]
        public void Should_Get_Users()
        {
            var output = _userAppService.GetUsers();
            output.Items.Count.ShouldBe(2); //Since initial data has 2 users.
            output.Items.FirstOrDefault(i => i.UserName == "admin").ShouldNotBe(null); //Since initial data has admin user
        }
    }
}
