using System.Threading.Tasks;
using Abp.IdentityFramework;
using Abp.Threading;
using Abp.Zero.SampleApp.Roles;
using Abp.Zero.SampleApp.Users;
using Microsoft.AspNet.Identity;
using Shouldly;
using Xunit;

namespace Abp.Zero.SampleApp.Tests.Users
{
    public class UserManager_Permission_Tests : SampleAppTestBase
    {
        private readonly User _testUser;
        private readonly Role _role1;
        private readonly Role _role2;

        public UserManager_Permission_Tests()
        {
            _role1 = CreateRole("Role1");
            _role2 = CreateRole("Role2");
            _testUser = CreateUser("TestUser");
            UserManager.AddToRoles(_testUser.Id, _role1.Name, _role2.Name).CheckErrors();
        }

        [Fact]
        public async Task Should_Not_Be_Granted_With_No_Permission_Setting()
        {
            (await IsGrantedAsync("Permission1")).ShouldBe(false);
        }

        [Fact]
        public async Task Should_Be_Granted_If_One_Of_Roles_Is_Granted()
        {
            await GrantPermissionAsync(_role1, "Permission1");
            (await IsGrantedAsync("Permission1")).ShouldBe(true);
        }

        [Fact]
        public async Task Should_Not_Be_Granted_After_Granted_Role_Is_Deleted()
        {
            //Not granted initially
            (await IsGrantedAsync("Permission1")).ShouldBe(false);

            //Grant one role of the user
            await GrantPermissionAsync(_role1, "Permission1");

            //Now, should be granted
            (await IsGrantedAsync("Permission1")).ShouldBe(true);

            //Delete the role
            await RoleManager.DeleteAsync(_role1);

            //Now, should not be granted
            (await IsGrantedAsync("Permission1")).ShouldBe(false);
        }

        [Fact]
        public async Task Should_Be_Granted_If_Granted_For_User()
        {
            await GrantPermissionAsync(_testUser, "Permission1");
            (await IsGrantedAsync("Permission1")).ShouldBe(true);
        }

        [Fact]
        public async Task Should_Not_Be_Granted_If_Prohibited_For_User()
        {
            //Permission3 is granted by default, but prohibiting for this user
            await ProhibitPermissionAsync(_testUser, "Permission3");
            (await IsGrantedAsync("Permission3")).ShouldBe(false);
        }

        [Fact]
        public async Task Should_Not_Be_Granted_If_Granted_For_Role_But_Prohibited_For_User()
        {
            await GrantPermissionAsync(_role2, "Permission1");
            await ProhibitPermissionAsync(_testUser, "Permission1");
            (await IsGrantedAsync("Permission1")).ShouldBe(false);
        }

        [Fact]
        public async Task SetGrantedPermissions_And_Reset_Test()
        {
            await GrantPermissionAsync(_role1, "Permission1");
            await GrantPermissionAsync(_role2, "Permission2");

            //Set permissions

            await UserManager.SetGrantedPermissionsAsync(
                _testUser,
                new[]
                {
                    PermissionManager.GetPermission("Permission1"),
                    PermissionManager.GetPermission("Permission4")
                });
            
            (await IsGrantedAsync("Permission1")).ShouldBe(true);
            (await IsGrantedAsync("Permission2")).ShouldBe(false);
            (await IsGrantedAsync("Permission3")).ShouldBe(false);
            (await IsGrantedAsync("Permission4")).ShouldBe(true);

            //Reset user-specific permissions

            await UserManager.ResetAllPermissionsAsync(_testUser);

            (await IsGrantedAsync("Permission1")).ShouldBe(true); //Role1 has Permission1
            (await IsGrantedAsync("Permission2")).ShouldBe(true); //Role2 has Permission2
            (await IsGrantedAsync("Permission3")).ShouldBe(false);
            (await IsGrantedAsync("Permission4")).ShouldBe(false);
        }

        [Fact]
        public async Task ProhibitAllPermissions_Test()
        {
            await GrantPermissionAsync(_role1, "Permission1");
            await UserManager.ProhibitAllPermissionsAsync(_testUser);
            foreach (var permission in PermissionManager.GetAllPermissions())
            {
                (await IsGrantedAsync(permission.Name)).ShouldBe(false);
            }
        }

        private async Task<bool> IsGrantedAsync(string permissionName)
        {
            return (await PermissionChecker.IsGrantedAsync(_testUser.ToUserIdentifier(), permissionName));
        }
    }
}
