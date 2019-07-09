using System.Collections.Generic;
using System.Linq;
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
                Component.For<IFeatureDependencyContext, FeatureDependencyContext>().UsingFactoryMethod(() => new FeatureDependencyContext(LocalIocManager, Substitute.For<IFeatureChecker>())),
                Component.For<MyAuthorizationProvider1>().LifestyleTransient(),
                Component.For<MyAuthorizationProvider2>().LifestyleTransient()
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

            userManagement.RemoveChildPermission(userManagement.Children.FirstOrDefault()?.Name);
            userManagement.Children.Count.ShouldBe(0);

            permissionManager.RemovePermission("Abp.Zero.Administration");
            permissionManager.GetPermissionOrNull("Abp.Zero.Administration").ShouldBe(null);
        }
        [Fact]
        public void Should_Manage_Permission_With_Custom_Properties()
        {
            var authorizationConfiguration = new AuthorizationConfiguration();
            authorizationConfiguration.Providers.Add<MyAuthorizationProviderWithCustomProperties>();

            LocalIocManager.IocContainer.Register(
                Component.For<IFeatureDependencyContext, FeatureDependencyContext>()
                    .UsingFactoryMethod(() => new FeatureDependencyContext(LocalIocManager, Substitute.For<IFeatureChecker>())),
                Component.For<MyAuthorizationProviderWithCustomProperties>().LifestyleTransient()
            );

            var permissionManager = new PermissionManager(LocalIocManager, authorizationConfiguration);
            permissionManager.Initialize();

            permissionManager.GetAllPermissions().Count.ShouldBe(4);

            var customPermission = permissionManager.GetPermissionOrNull("Abp.Zero.MyCustomPermission");
            customPermission.ShouldNotBe(null);
            customPermission.Children.Count.ShouldBe(2);

            customPermission.Properties.Count.ShouldBe(2);
            customPermission["MyProp1"].ShouldBe("Test");
            ((MyAuthorizationProviderWithCustomProperties.MyTestPropertyClass)customPermission["MyProp2"]).Prop1.ShouldBe("Test");

            //its not exist
            customPermission["MyProp3"].ShouldBeNull();

            customPermission.Children[0]["MyProp1"].ShouldBeNull();
            customPermission.Children[1]["MyProp1"].ShouldBe("TestChild");



            var customPermission2 = permissionManager.GetPermissionOrNull("Abp.Zero.MyCustomPermission2");
            customPermission2.ShouldNotBe(null);
            customPermission2.Children.Count.ShouldBe(0);

            customPermission2.Properties.Count.ShouldBe(0);
            customPermission2["MyProp1"].ShouldBeNull();

            customPermission2["MyProp1"] = "Test";

            var customPermission21 = permissionManager.GetPermissionOrNull("Abp.Zero.MyCustomPermission2");
            customPermission2.ShouldBeSameAs(customPermission21);

            customPermission21["MyProp1"].ShouldBe("Test");

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

    public class MyAuthorizationProviderWithCustomProperties : AuthorizationProvider
    {
        public class MyTestPropertyClass
        {
            public string Prop1 { get; set; }
        }
        public override void SetPermissions(IPermissionDefinitionContext context)
        {
            var myPermission = context.CreatePermission("Abp.Zero.MyCustomPermission",
                new FixedLocalizableString("Administration"),
                properties: new Dictionary<string, object>()
                {
                    {"MyProp1", "Test"},
                    {"MyProp2", new MyTestPropertyClass {Prop1 = "Test"}}
                }
            );
            //add children to permission
            myPermission.CreateChildPermission("Abp.Zero.MyCustomChildPermission",
                new FixedLocalizableString("Role management")
             );
            myPermission.CreateChildPermission("Abp.Zero.MyCustomChildPermission2",
                new FixedLocalizableString("Role management"),
                properties: new Dictionary<string, object>()
                {
                        {"MyProp1", "TestChild"},
                        {"MyProp2", new MyTestPropertyClass {Prop1 = "TestChild"}}
                });

            var myPermission2 = context.CreatePermission("Abp.Zero.MyCustomPermission2",
                new FixedLocalizableString("Administration")
            );
        }
    }
}
