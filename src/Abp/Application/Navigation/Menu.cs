using System;
using System.Collections.Generic;
using Abp.Localization;

namespace Abp.Application.Navigation
{
    /// <summary>
    /// Represents a navigation menu for an application.
    /// </summary>
    public class Menu : IHasMenuItems
    {
        /// <summary>
        /// Unique name of the menu in the application. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Display name of the menu.
        /// </summary>
        public ILocalizableString DisplayName { get; set; }

        /// <summary>
        /// Menu items (first level).
        /// </summary>
        public IList<MenuItem> Items { get; set; }

        /// <summary>
        /// Creates a new <see cref="Menu"/> object.
        /// </summary>
        /// <param name="name">Unique name of the menu</param>
        /// <param name="displayName">Display name of the menu</param>
        public Menu(string name, ILocalizableString displayName)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "Menu name can not be empty or null.");
            }

            if (displayName == null)
            {
                throw new ArgumentNullException("displayName", "Display name of the menu can not be null.");
            }

            Name = name;
            DisplayName = displayName;

            Items = new List<MenuItem>();
        }

        /// <summary>
        /// Adds a <see cref="MenuItem"/> to <see cref="Items"/>.
        /// </summary>
        /// <param name="menuItem"><see cref="MenuItem"/> to be added</param>
        /// <returns>This <see cref="Menu"/> object</returns>
        public Menu AddItem(MenuItem menuItem)
        {
            Items.Add(menuItem);
            return this;
        }
    }
}
