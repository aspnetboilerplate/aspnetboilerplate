using Abp.Application.Features;
using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using Castle.MicroKernel.Registration;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Tests.Authorization
{
    public class PermissionManagerTester : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Test_PermissionManager()
        {
            var authorizationConfiguration = new AuthorizationConfiguration();
            authorizationConfiguration.Providers.Add<MyAuthorizationProvider1>();
            authorizationConfiguration.Providers.Add<MyAuthorizationProvider2>();

            LocalIocManager.IocContainer.Register(
                Component.For<IFeatureDependencyContext, FeatureDependencyContext>().UsingFactoryMethod(() => new FeatureDependencyContext(LocalIocManager, Substitute.For<IFeatureChecker>()))
                );

            var permissionManager = new PermissionManager(LocalIocManager, authorizationConfiguration);
            permissionManager.Initialize();

            permissionManager.GetAllPermissions().Count.ShouldBe(5);

            var userManagement = permissionManager.GetPermissionOrNull("Abp.Zero.Administration.UserManagement");
            userManagement.ShouldNotBe(null);
            userManagement.Children.Count.ShouldBe(1);

            var changePermissions = permissionManager.GetPermissionOrNull("Abp.Zero.Administration.UserManagement.ChangePermissions");
            changePermissions.ShouldNotBe(null);
            changePermissions.Parent.ShouldBeSameAs(userManagement);

            permissionManager.GetPermissionOrNull("NonExistingPermissionName").ShouldBe(null);
        }
    }

    public class MyAuthorizationProvider1 : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //Create a root permission group for 'Administration' permissions
            var administration = context.CreatePermission("Abp.Zero.Administration", new FixedLocalizableString("Administration"));

            //Create 'User management' permission under 'Administration' group
            var userManagement = administration.CreateChildPermission("Abp.Zero.Administration.UserManagement", new FixedLocalizableString("User management"));

            //Create 'Change permissions' (to be able to change permissions of a user) permission as child of 'User management' permission.
            userManagement.CreateChildPermission("Abp.Zero.Administration.UserManagement.ChangePermissions", new FixedLocalizableString("Change permissions"));
        }
    }

    public class MyAuthorizationProvider2 : AuthorizationProvider
    {
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            //Get existing root permission group 'Administration'
            var administration = context.GetPermissionOrNull("Abp.Zero.Administration");
            administration.ShouldNotBe(null);

            //Create 'Role management' permission under 'Administration' group
            var roleManegement = administration.CreateChildPermission("Abp.Zero.Administration.RoleManagement", new FixedLocalizableString("Role management"));

            //Create 'Create role' (to be able to create a new role) permission  as child of 'Role management' permission.
            roleManegement.CreateChildPermission("Abp.Zero.Administration.RoleManagement.CreateRole", new FixedLocalizableString("Create role"));
        }
    }
}
