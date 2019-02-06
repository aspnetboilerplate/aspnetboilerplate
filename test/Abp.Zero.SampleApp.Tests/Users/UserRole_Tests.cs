using System.Threading.Tasks;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Uow;
using Abp.IdentityFramework;
using Abp.Organizations;
using Abp.Zero.SampleApp.MultiTenancy;
using Abp.Zero.SampleApp.Roles;
using Abp.Zero.SampleApp.Users;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Users
{
    public class UserRole_Tests : SampleAppTestBase
    {
        public UserRole_Tests()
        {
            UsingDbContext(
                context =>
                {
                    var tenant1 = context.Tenants.Add(new Tenant("tenant1", "Tenant one"));
                    context.SaveChanges();

                    AbpSession.TenantId = tenant1.Id;

                    var user1 = new User
                    {
                        TenantId = AbpSession.TenantId,
                        UserName = "user1",
                        Name = "User",
                        Surname = "One",
                        EmailAddress = "user-one@aspnetboilerplate.com",
                        IsEmailConfirmed = true,
                        Password = "AM4OLBpptxBYmM79lGOX9egzZk3vIQU3d/gFCJzaBjAPXzYIK3tQ2N7X4fcrHtElTw=="
                        //123qwe
                    };

                    user1.SetNormalizedNames();

                    context.Users.Add(user1);
                    context.SaveChanges();

                    var role1 = context.Roles.Add(new Role(AbpSession.TenantId, "role1", "Role 1"));
                    var role2 = context.Roles.Add(new Role(AbpSession.TenantId, "role2", "Role 2"));
                    var role3 = context.Roles.Add(new Role(AbpSession.TenantId, "organizationUnitRole", "Organization Unit Role"));
                    context.SaveChanges();

                    var ou1 = context.OrganizationUnits.Add(new OrganizationUnit
                    {
                        TenantId = AbpSession.TenantId,
                        DisplayName = "ou 1",
                        Code = "ou1"
                    });
                    context.SaveChanges();

                    context.UserOrganizationUnits.Add(new UserOrganizationUnit(AbpSession.TenantId, user1.Id, ou1.Id));
                    context.OrganizationUnitRoles.Add(new OrganizationUnitRole(AbpSession.TenantId, role3.Id, ou1.Id));
                    context.UserRoles.Add(new UserRole(AbpSession.TenantId, user1.Id, role1.Id));
                });
        }

        [Fact]
        public async Task Should_Change_Roles()
        {
            var unitOfWorkManager = LocalIocManager.Resolve<IUnitOfWorkManager>();
            using (var uow = unitOfWorkManager.Begin())
            {
                var user = await UserManager.FindByNameAsync("user1");

                //Check initial role assignments
                var roles = await UserManager.GetRolesAsync(user.Id);
                roles.ShouldContain("role1");
                roles.ShouldNotContain("role2");

                //Delete all role assignments
                (await UserManager.RemoveFromRolesAsync(user.Id, "role1")).CheckErrors();
                await unitOfWorkManager.Current.SaveChangesAsync();

                //Check role assignments again
                roles = await UserManager.GetRolesAsync(user.Id);
                roles.ShouldNotContain("role1");
                roles.ShouldNotContain("role2");

                //Add to roles
                (await UserManager.AddToRolesAsync(user.Id, "role1", "role2")).CheckErrors();
                await unitOfWorkManager.Current.SaveChangesAsync();

                //Check role assignments again
                roles = await UserManager.GetRolesAsync(user.Id);
                roles.ShouldContain("role1");
                roles.ShouldContain("role2");

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task Should_Get_User_OrganizationUnit_Roles()
        {
            var unitOfWorkManager = LocalIocManager.Resolve<IUnitOfWorkManager>();
            using (var uow = unitOfWorkManager.Begin())
            {
                var user = await UserManager.FindByNameAsync("user1");

                //Check initial role assignments
                var roles = await UserManager.GetRolesAsync(user.Id);
                roles.ShouldContain("role1");
                roles.ShouldNotContain("role2");
                roles.ShouldContain("organizationUnitRole");

                await uow.CompleteAsync();
            }
        }
    }
}