using System.Collections.Generic;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Dependency;

namespace Abp.Application.Navigation
{
    internal class UserNavigationManager : IUserNavigationManager, ISingletonDependency
    {
        private readonly IPermissionManager _permissionManager;
        private readonly INavigationManager _navigationManager;

        public UserNavigationManager(IPermissionManager permissionManager, INavigationManager navigationManager)
        {
            _permissionManager = permissionManager;
            _navigationManager = navigationManager;
        }

        public UserMenu GetUserMenu(string menuName, long userId)
        {
            var menuDefinition = _navigationManager.Menus.GetOrDefault(menuName);
            if (menuDefinition == null)
            {
                throw new AbpException("There is no menu with given name: " + menuName);
            }

            var userMenu = new UserMenu(menuDefinition);
            FillUserMenuItems(userId, menuDefinition.Items, userMenu.Items);
            return userMenu;
        }

        private int FillUserMenuItems(long userId, IList<MenuItemDefinition> menuItemDefinitions, IList<UserMenuItem> userMenuItems)
        {
            var addedMenuItemCount = 0;

            foreach (var menuItemDefinition in menuItemDefinitions)
            {
                if (string.IsNullOrEmpty(menuItemDefinition.RequiredPermissionName) ||
                    _permissionManager.IsGranted(userId, menuItemDefinition.RequiredPermissionName))
                {
                    var userMenuItem = new UserMenuItem(menuItemDefinition);
                    if (menuItemDefinition.Items.Count <= 0 || FillUserMenuItems(userId, menuItemDefinition.Items, userMenuItem.Items) > 0)
                    {
                        userMenuItems.Add(userMenuItem);
                        ++addedMenuItemCount;
                    }
                }
            }

            return addedMenuItemCount;
        }
    }
}
