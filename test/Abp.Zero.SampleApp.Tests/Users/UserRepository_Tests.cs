using Abp.Domain.Repositories;
using Abp.Zero.SampleApp.Users;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Users
{
    public class UserRepository_Tests : SampleAppTestBase
    {
        [Fact]
        public void Should_Insert_And_Retrieve_User()
        {
            var userRepository = LocalIocManager.Resolve<IRepository<User, long>>();

            userRepository.FirstOrDefault(u => u.EmailAddress == "admin@aspnetboilerplate.com").ShouldBe(null);

            var user = new User
            {
                TenantId = null,
                UserName = "admin",
                Name = "System",
                Surname = "Administrator",
                EmailAddress = "admin@aspnetboilerplate.com",
                IsEmailConfirmed = true,
                Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw==" //123qwe
            };

            user.SetNormalizedNames();
            userRepository.Insert(user);

            userRepository.FirstOrDefault(u => u.EmailAddress == "admin@aspnetboilerplate.com").ShouldNotBe(null);
        }
    }
}
