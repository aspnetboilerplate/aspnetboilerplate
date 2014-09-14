using System.Collections.Generic;
using Abp.Application.Authorization.Permissions;
using Abp.Localization;
using NUnit.Framework;

namespace Abp.Application.Tests.Permissions
{
    [TestFixture]
    public class PermissionManagerTester
    {
        [Test]
        public void Test_PermissionManager()
        {
            var permissionManager = new PermissionManager(new MyPermissionDefinitionProviderFinder());

            Assert.AreEqual(1, permissionManager.GetRootPermissionGroups().Count);
            Assert.AreEqual(4, permissionManager.GetAllPermissions().Count);

            var userManagement = permissionManager.GetPermissionOrNull("Abp.Zero.Administration.UserManagement");
            Assert.NotNull(userManagement);
            Assert.AreEqual(1, userManagement.Children.Count);

            var changePermissions = permissionManager.GetPermissionOrNull("Abp.Zero.Administration.UserManagement.ChangePermissions");
            Assert.NotNull(changePermissions);

            Assert.AreSame(userManagement, changePermissions.Parent);

            Assert.Null(permissionManager.GetPermissionOrNull("NonExistingPermissionName"));
        }
    }

    public class MyPermissionDefinitionProviderFinder : IPermissionDefinitionProviderFinder
    {
        public IEnumerable<IPermissionDefinitionProvider> GetPermissionProviders()
        {
            return new IPermissionDefinitionProvider[]
                   {
                       new MyPermissionProvider1(),
                       new MyPermissionProvider2() 
                   };
        }
    }

    public class MyPermissionProvider1 : IPermissionDefinitionProvider
    {
        public void DefinePermissions(IPermissionDefinitionContext context)
        {
            //Create a root permission group for 'Administration' permissions
            var administration = context.CreateRootGroup("Abp.Zero.Administration", new LocalizableString("Administration", "AbpZero"));

            //Create 'User management' permission under 'Administration' group
            var userManagement = administration.CreatePermission("Abp.Zero.Administration.UserManagement", new LocalizableString("UserManagement", "AbpZero"));

            //Create 'Change permissions' (to be able to change permissions of a user) permission as child of 'User management' permission.
            userManagement.CreateChildPermission("Abp.Zero.Administration.UserManagement.ChangePermissions", new LocalizableString("ChangePermissions", "AbpZero"));
        }
    }

    public class MyPermissionProvider2 : IPermissionDefinitionProvider
    {
        public void DefinePermissions(IPermissionDefinitionContext context)
        {
            //Get existing root permission group 'Administration'
            var administration = context.GetRootGroupOrNull("Abp.Zero.Administration");

            //Create 'Role management' permission under 'Administration' group
            var roleManegement = administration.CreatePermission("Abp.Zero.Administration.RoleManagement", new LocalizableString("RoleManagement", "AbpZero"));

            //Create 'Create role' (to be able to create a new role) permission  as child of 'Role management' permission.
            roleManegement.CreateChildPermission("Abp.Zero.Administration.RoleManagement.CreateRole", new LocalizableString("CreateRole", "AbpZero"));
        }
    }
}
