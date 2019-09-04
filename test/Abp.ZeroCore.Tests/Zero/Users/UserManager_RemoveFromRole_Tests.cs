using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace Abp.Zero.Users
{
    public class UserManager_RemoveFromRole_Tests : AbpZeroTestBase
    {
        [Fact]
        public async Task RemoveFromRoleAsync_Test()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var userManager = LocalIocManager.Resolve<UserManager>();
                var roleManager = LocalIocManager.Resolve<RoleManager>();

                var adminUser = await userManager.FindByNameAsync("admin");
                var managerRole = await roleManager.FindByNameAsync("MANAGER");

                await userManager.SetRolesAsync(adminUser, new[] {managerRole.Name});

                await uow.CompleteAsync();
            }

            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var userManager = LocalIocManager.Resolve<UserManager>();

                var adminUser = await userManager.FindByNameAsync("admin");

                await userManager.RemoveFromRolesAsync(adminUser, new[] { "MANAGER" });

                var adminRoles = await userManager.GetRolesAsync(adminUser);
                adminRoles.Count.ShouldBe(0);

                await uow.CompleteAsync();
            }
        }
    }
}
