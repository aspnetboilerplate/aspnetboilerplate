using Abp.Application.Navigation;
using Abp.Authorization;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Localization;
using NSubstitute;

namespace Abp.Tests.Application.Navigation
{
    internal class NavigationTestCase
    {
        public NavigationManager NavigationManager { get; private set; }

        public UserNavigationManager UserNavigationManager { get; private set; }

        private readonly IIocManager _iocManager;

        public NavigationTestCase()
            : this(new IocManager())
        {
        }

        public NavigationTestCase(IIocManager iocManager)
        {
            _iocManager = iocManager;
            Initialize();
        }

        private void Initialize()
        {
            //Navigation providers should be registered
            _iocManager.Register<MyNavigationProvider1>();
            _iocManager.Register<MyNavigationProvider2>();

            //Preparing navigation configuration
            var configuration = new NavigationConfiguration();
            configuration.Providers.Add<MyNavigationProvider1>();
            configuration.Providers.Add<MyNavigationProvider2>();

            //Initializing navigation manager
            NavigationManager = new NavigationManager(_iocManager, configuration);
            NavigationManager.Initialize();

            //Create user navigation manager to test
            UserNavigationManager = new UserNavigationManager(CreateMockPermissionManager(), NavigationManager);
        }

        private static IPermissionManager CreateMockPermissionManager()
        {
            var permissionManager = Substitute.For<IPermissionManager>();
            permissionManager.IsGranted(1, "Abp.Zero.UserManagement").Returns(true);
            permissionManager.IsGranted(1, "Abp.Zero.RoleManagement").Returns(false);
            return permissionManager;
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
}