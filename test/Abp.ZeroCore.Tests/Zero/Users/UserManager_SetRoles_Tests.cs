using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Uow;
using Abp.ZeroCore.SampleApp.Core;
using Shouldly;
using Xunit;

namespace Abp.Zero.Users
{
    public class UserManager_SetRoles_Tests : AbpZeroTestBase
    {
        [Fact]
        public async Task SetRolesAsync_Test()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var userManager = LocalIocManager.Resolve<UserManager>();
                var roleManager = LocalIocManager.Resolve<RoleManager>();

                var adminUser = await userManager.FindByNameAsync("admin");
                var managerRole = await roleManager.FindByNameAsync("MANAGER");

                await userManager.SetRolesAsync(adminUser, new[] { managerRole.Name });
                await roleManager.DeleteAsync(managerRole);

                await uow.CompleteAsync();
            }

            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var userManager = LocalIocManager.Resolve<UserManager>();
                var roleManager = LocalIocManager.Resolve<RoleManager>();

                var adminUser = await userManager.FindByNameAsync("admin");
                var useRole = await roleManager.FindByNameAsync("user");

                await userManager.SetRolesAsync(adminUser, new[] { useRole.Name });

                var adminRoles = await userManager.GetRolesAsync(adminUser);

                adminRoles.Count.ShouldBe(1);
                adminRoles.ShouldContain(x => x == "User");

                await uow.CompleteAsync();
            }
        }
    }
}
