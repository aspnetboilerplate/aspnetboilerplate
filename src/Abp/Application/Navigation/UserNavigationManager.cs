using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

        public UserMenu GetMenu(string menuName, long? userId)
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

        public IReadOnlyList<UserMenu> GetMenus(long? userId)
        {
            return _navigationManager.Menus.Values.Select(m => GetMenu(m.Name, userId)).ToImmutableList();
        }

        private int FillUserMenuItems(long? userId, IList<MenuItemDefinition> menuItemDefinitions, IList<UserMenuItem> userMenuItems)
        {
            var addedMenuItemCount = 0;

            foreach (var menuItemDefinition in menuItemDefinitions)
            {
                if (menuItemDefinition.RequiresAuthentication && !userId.HasValue)
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(menuItemDefinition.RequiredPermissionName) && (!userId.HasValue || !_permissionManager.IsGranted(userId.Value, menuItemDefinition.RequiredPermissionName)))
                {
                    continue;
                }

                var userMenuItem = new UserMenuItem(menuItemDefinition);
                if (menuItemDefinition.IsLeaf || FillUserMenuItems(userId, menuItemDefinition.Items, userMenuItem.Items) > 0)
                {
                    userMenuItems.Add(userMenuItem);
                    ++addedMenuItemCount;
                }
            }

            return addedMenuItemCount;
        }
    }
}
