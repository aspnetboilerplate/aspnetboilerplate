using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Zero.SampleApp.Roles;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Roles
{
    public class RoleManager_Tests : SampleAppTestBase
    {
        [Fact]
        public async Task Should_Create_And_Retrieve_Role()
        {
            await CreateRole("Role1");

            var role1Retrieved = await RoleManager.FindByNameAsync("Role1");
            role1Retrieved.ShouldNotBe(null);
            role1Retrieved.Name.ShouldBe("Role1");
        }

        [Fact]
        public async Task Should_Not_Create_For_Duplicate_Name_Or_DisplayName()
        {
            //Create a role and check
            await CreateRole("Role1", "Role One");
            (await RoleManager.FindByNameAsync("Role1")).ShouldNotBe(null);

            //Create with same name
            (await RoleManager.CreateAsync(new Role(null, "Role1", "Role Uno"))).Succeeded.ShouldBe(false);
            (await RoleManager.CreateAsync(new Role(null, "Role2", "Role One"))).Succeeded.ShouldBe(false);
        }

        [Fact]
        public async Task PermissionTests()
        {
            var role1 = await CreateRole("Role1");

            (await RoleManager.IsGrantedAsync(role1.Id, PermissionManager.GetPermission("Permission1"))).ShouldBe(false);
            (await RoleManager.IsGrantedAsync(role1.Id, PermissionManager.GetPermission("Permission3"))).ShouldBe(true);

            await GrantPermissionAsync(role1, "Permission1");
            await ProhibitPermissionAsync(role1, "Permission1");

            await ProhibitPermissionAsync(role1, "Permission3");
            await GrantPermissionAsync(role1, "Permission3");

            await GrantPermissionAsync(role1, "Permission1");
            await ProhibitPermissionAsync(role1, "Permission3");

            var grantedPermissions = await RoleManager.GetGrantedPermissionsAsync(role1);
            grantedPermissions.Count.ShouldBe(2);
            grantedPermissions.ShouldContain(p => p.Name == "Permission1");
            grantedPermissions.ShouldContain(p => p.Name == "Permission4");

            var newPermissionList = new List<Permission>
                                    {
                                        PermissionManager.GetPermission("Permission1"),
                                        PermissionManager.GetPermission("Permission2"),
                                        PermissionManager.GetPermission("Permission3")
                                    };

            await RoleManager.SetGrantedPermissionsAsync(role1, newPermissionList);

            grantedPermissions = await RoleManager.GetGrantedPermissionsAsync(role1);

            grantedPermissions.Count.ShouldBe(3);
            grantedPermissions.ShouldContain(p => p.Name == "Permission1");
            grantedPermissions.ShouldContain(p => p.Name == "Permission2");
            grantedPermissions.ShouldContain(p => p.Name == "Permission3");
        }
    }
}