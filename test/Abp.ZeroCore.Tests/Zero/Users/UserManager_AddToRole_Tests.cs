using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace Abp.Zero.Users
{
    public class UserManager_AddToRole_Tests : AbpZeroTestBase
    {
        [Fact]
        public async Task AddToRoleAsync_Test()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
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

                var userManager = LocalIocManager.Resolve<UserManager>();
                await userManager.CreateAsync(user);
                await userManager.AddToRoleAsync(user, "ADMIN");

                user.Roles.Count.ShouldBe(1);

                await uow.CompleteAsync();
            }
        }
    }
}
