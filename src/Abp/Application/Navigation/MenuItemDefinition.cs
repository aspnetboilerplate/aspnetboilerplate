using System;
using System.Collections.Generic;
using Abp.Localization;

namespace Abp.Application.Navigation
{
    /// <summary>
    /// Represents an item in a <see cref="MenuDefinition"/>.
    /// </summary>
    public class MenuItemDefinition : IHasMenuItemDefinitions
    {
        /// <summary>
        /// Unique name of the menu item in the application. 
        /// Can be used to find this menu item later.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Icon of the menu item if exists. Optional.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Display name of the menu item. Required.
        /// </summary>
        public ILocalizableString DisplayName { get; private set; }

        /// <summary>
        /// The URL to navigate when this menu item is selected. Optional.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Sub items of this menu item. Optional.
        /// </summary>
        public IList<MenuItemDefinition> Items { get; private set; }

        /// <summary>
        /// A permission name. Optional.
        /// </summary>
        public string RequiredPermissionName { get; set; }

        /// <summary>
        /// Creates a new <see cref="MenuItemDefinition"/> object.
        /// </summary>
        public MenuItemDefinition(string name, ILocalizableString displayName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "Menu item's name can not be null or empty");
            }

            Name = name;
            DisplayName = displayName;
            Items = new List<MenuItemDefinition>();
        }

        /// <summary>
        /// Adds a <see cref="MenuItemDefinition"/> to <see cref="Items"/>.
        /// </summary>
        /// <param name="menuItem"><see cref="MenuItemDefinition"/> to be added</param>
        /// <returns>This <see cref="MenuItemDefinition"/> object</returns>
        public MenuItemDefinition AddItem(MenuItemDefinition menuItem)
        {
            Items.Add(menuItem);
            return this;
        }
    }
}