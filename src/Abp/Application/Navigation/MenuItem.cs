using System.Collections.Generic;
using Abp.Localization;

namespace Abp.Application.Navigation
{
    /// <summary>
    /// Represents an item in a <see cref="Menu"/>.
    /// </summary>
    public class MenuItem : IHasMenuItems
    {
        /// <summary>
        /// Unique name of the menu item in the application. 
        /// Can be used to find this menu item later. Optional.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Icon of the menu item if exists. Optional.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// Display name of the menu item. Required.
        /// </summary>
        public ILocalizableString DisplayName { get; set; }

        /// <summary>
        /// The URL to navigate when this menu item is selected. Optional.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Sub items of this menu item. Optional.
        /// </summary>
        public IList<MenuItem> Items { get; private set; }

        /// <summary>
        /// A permission name. Optional.
        /// </summary>
        public string RequiredPermissionName { get; set; }

        //public Dictionary<string, string> Settings { get; set; } //Maybe implemented later?

        /// <summary>
        /// Creates a new <see cref="MenuItem"/> object.
        /// </summary>
        public MenuItem(ILocalizableString displayName)
        {
            //Settings = new Dictionary<string, string>();
            Items = new List<MenuItem>();
            DisplayName = displayName;
        }

        /// <summary>
        /// Adds a <see cref="MenuItem"/> to <see cref="Items"/>.
        /// </summary>
        /// <param name="menuItem"><see cref="MenuItem"/> to be added</param>
        /// <returns>This <see cref="MenuItem"/> object</returns>
        public MenuItem AddItem(MenuItem menuItem)
        {
            Items.Add(menuItem);
            return this;
        }
    }
}