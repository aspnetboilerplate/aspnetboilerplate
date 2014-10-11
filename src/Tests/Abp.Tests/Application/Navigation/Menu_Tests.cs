using System.Linq;
using Abp.Application.Navigation;
using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Localization;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.Tests.Application.Navigation
{
    public class Menu_Tests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void Test1()
        {
            //Navigation providers should be registered
            LocalIocManager.Register<MyNavigationProvider1>();
            LocalIocManager.Register<MyNavigationProvider2>();

            //Preparing navigation configuration
            var configuration = new NavigationConfiguration();
            configuration.Providers.Add<MyNavigationProvider1>();
            configuration.Providers.Add<MyNavigationProvider2>();

            //Initializing navigation manager
            var navigationManager = new NavigationManager(LocalIocManager, configuration);
            navigationManager.Initialize();

            //Check created menu definitions
            var mainMenuDefinition = navigationManager.MainMenu;
            mainMenuDefinition.Items.Count.ShouldBe(1);

            var adminMenuItemDefinition = mainMenuDefinition.GetItemByNameOrNull("Abp.Zero.Administration");
            adminMenuItemDefinition.ShouldNotBe(null);
            adminMenuItemDefinition.Items.Count.ShouldBe(3);

            var userNavigationManager = new UserNavigationManager(CreateMockPermissionManager(), navigationManager);
            var userMenu = userNavigationManager.GetUserMenu(mainMenuDefinition.Name, 1);
            userMenu.Items.Count.ShouldBe(1);
            
            var userAdminMenu = userMenu.Items.FirstOrDefault(i => i.Name == "Abp.Zero.Administration");
            userAdminMenu.ShouldNotBe(null);

            userAdminMenu.Items.FirstOrDefault(i => i.Name == "Abp.Zero.Administration.User").ShouldNotBe(null);
            userAdminMenu.Items.FirstOrDefault(i => i.Name == "Abp.Zero.Administration.Role").ShouldBe(null);
            userAdminMenu.Items.FirstOrDefault(i => i.Name == "Abp.Zero.Administration.Setting").ShouldNotBe(null);
        }

        private IPermissionManager CreateMockPermissionManager()
        {
            var permissionManager = Substitute.For<IPermissionManager>();
            permissionManager.IsGranted(1, "Abp.Zero.UserManagement").Returns(true);
            permissionManager.IsGranted(1, "Abp.Zero.RoleManagement").Returns(false);
            return permissionManager;
        }
    }

    public class MyNavigationProvider1 : INavigationProvider
    {
        public void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu.AddItem(
                new MenuItemDefinition("Abp.Zero.Administration", new FixedLocalizableString("Administration"))
                {
                    Icon = "fa fa-asterisk",
                    Name = "Abp.Zero.Administration"
                }
                    .AddItem(new MenuItemDefinition("Abp.Zero.Administration.User", new FixedLocalizableString("User management"))
                             {
                                 Icon = "fa fa-users",
                                 Url = "#/admin/users",
                                 RequiredPermissionName = "Abp.Zero.UserManagement"
                             })
                    .AddItem(new MenuItemDefinition("Abp.Zero.Administration.Role", new FixedLocalizableString("Role management"))
                             {
                                 Icon = "fa fa-star-o",
                                 Url = "#/admin/roles",
                                 RequiredPermissionName = "Abp.Zero.RoleManagement"
                             })
                );
        }
    }

    public class MyNavigationProvider2 : INavigationProvider
    {
        public void SetNavigation(INavigationProviderContext context)
        {
            var adminMenu = context.Manager.MainMenu.GetItemByName("Abp.Zero.Administration");
            adminMenu.AddItem(new MenuItemDefinition("Abp.Zero.Administration.Setting", new FixedLocalizableString("Setting management"))
                              {
                                  Icon = "fa fa-cog",
                                  Url = "#/admin/settings"
                              });
        }
    }
}
