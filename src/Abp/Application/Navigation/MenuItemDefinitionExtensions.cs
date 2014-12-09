using System.Collections.Generic;
using System.Linq;

namespace Abp.Application.Navigation
{
    /// <summary>
    /// Extension methods for <see cref="MenuItemDefinition"/>.
    /// </summary>
    public static class MenuItemDefinitionExtensions
    {
        /// <summary>
        /// Moves a menu item to top in the list.
        /// </summary>
        /// <param name="menuItems">List of menu items</param>
        /// <param name="menuItemName">Name of the menu item to move</param>
        public static void MoveMenuItemToTop(this IList<MenuItemDefinition> menuItems, string menuItemName)
        {
            var menuItem = GetMenuItem(menuItems, menuItemName);
            menuItems.Remove(menuItem);
            menuItems.Insert(0, menuItem);
        }

        /// <summary>
        /// Moves a menu item to bottom in the list.
        /// </summary>
        /// <param name="menuItems">List of menu items</param>
        /// <param name="menuItemName">Name of the menu item to move</param>
        public static void MoveMenuItemToBottom(this IList<MenuItemDefinition> menuItems, string menuItemName)
        {
            var menuItem = GetMenuItem(menuItems, menuItemName);
            menuItems.Remove(menuItem);
            menuItems.Insert(menuItems.Count, menuItem);
        }

        /// <summary>
        /// Moves a menu item in the list after another menu item in the list.
        /// </summary>
        /// <param name="menuItems">List of menu items</param>
        /// <param name="menuItemName">Name of the menu item to move</param>
        /// <param name="targetMenuItemName">Target menu item (to move before it)</param>
        public static void MoveMenuItemBefore(this IList<MenuItemDefinition> menuItems, string menuItemName, string targetMenuItemName)
        {
            var menuItem = GetMenuItem(menuItems, menuItemName);
            var targetMenuItem = GetMenuItem(menuItems, targetMenuItemName);
            menuItems.Remove(menuItem);
            menuItems.Insert(menuItems.IndexOf(targetMenuItem), menuItem);
        }

        /// <summary>
        /// Moves a menu item in the list before another menu item in the list.
        /// </summary>
        /// <param name="menuItems">List of menu items</param>
        /// <param name="menuItemName">Name of the menu item to move</param>
        /// <param name="targetMenuItemName">Target menu item (to move after it)</param>
        public static void MoveMenuItemAfter(this IList<MenuItemDefinition> menuItems, string menuItemName, string targetMenuItemName)
        {
            var menuItem = GetMenuItem(menuItems, menuItemName);
            var targetMenuItem = GetMenuItem(menuItems, targetMenuItemName);
            menuItems.Remove(menuItem);
            menuItems.Insert(menuItems.IndexOf(targetMenuItem) + 1, menuItem);
        }

        private static MenuItemDefinition GetMenuItem(IEnumerable<MenuItemDefinition> menuItems, string menuItemName)
        {
            var menuItem = menuItems.FirstOrDefault(i => i.Name == menuItemName);
            if (menuItem == null)
            {
                throw new AbpException("Can not find menu item: " + menuItemName);
            }

            return menuItem;
        }
    }
}