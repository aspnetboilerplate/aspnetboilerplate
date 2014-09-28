using System;
using System.Collections.Generic;
using Abp.Authorization;
using Abp.Dependency;
using Abp.Localization;
using Xunit;

namespace Abp.Tests.Authorization
{
    public class PermissionManagerTester
    {
        [Fact]
        public void Test_PermissionManager()
        {
            var permissionManager = new PermissionManager(IocManager.Instance, new MyPermissionProviderFinder());

            Assert.Equal(1, permissionManager.GetAllRootGroups().Count);
            Assert.Equal(4, permissionManager.GetAllPermissions().Count);

            var userManagement = permissionManager.GetPermissionOrNull("Abp.Zero.Administration.UserManagement");
            Assert.NotNull(userManagement);
            Assert.Equal(1, userManagement.Children.Count);

            var changePermissions = permissionManager.GetPermissionOrNull("Abp.Zero.Administration.UserManagement.ChangePermissions");
            Assert.NotNull(changePermissions);

            Assert.Same(userManagement, changePermissions.Parent);

            Assert.Null(permissionManager.GetPermissionOrNull("NonExistingPermissionName"));
        }
    }

    public class MyPermissionProviderFinder : IPermissionProviderFinder
    {
        public List<Type> FindAll()
        {
            return new List<Type>
                   {
                       typeof(MyPermissionProvider1),
                       typeof(MyPermissionProvider2)
                   };
        }
    }

    public class MyPermissionProvider1 : IPermissionProvider
    {
        public void DefinePermissions(IPermissionDefinitionContext context)
        {
            //Create a root permission group for 'Administration' permissions
            var administration = context.CreateRootGroup("Abp.Zero.Administration", new FixedLocalizableString("Administration"));

            //Create 'User management' permission under 'Administration' group
            var userManagement = administration.CreatePermission("Abp.Zero.Administration.UserManagement", new FixedLocalizableString("User management"));

            //Create 'Change permissions' (to be able to change permissions of a user) permission as child of 'User management' permission.
            userManagement.CreateChildPermission("Abp.Zero.Administration.UserManagement.ChangePermissions", new FixedLocalizableString("Change permissions"));
        }
    }

    public class MyPermissionProvider2 : IPermissionProvider
    {
        public void DefinePermissions(IPermissionDefinitionContext context)
        {
            //Get existing root permission group 'Administration'
            var administration = context.GetRootGroupOrNull("Abp.Zero.Administration");

            //Create 'Role management' permission under 'Administration' group
            var roleManegement = administration.CreatePermission("Abp.Zero.Administration.RoleManagement", new FixedLocalizableString("Role management"));

            //Create 'Create role' (to be able to create a new role) permission  as child of 'Role management' permission.
            roleManegement.CreateChildPermission("Abp.Zero.Administration.RoleManagement.CreateRole", new FixedLocalizableString("Create role"));
        }
    }
}
