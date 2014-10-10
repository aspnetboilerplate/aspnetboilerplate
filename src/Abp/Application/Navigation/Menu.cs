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

        //public Dictionary<string, string> Settings { get; set; } //Maybe implemented later?

        /// <summary>
        /// Creates a new <see cref="Menu"/> object.
        /// </summary>
        public Menu()
        {
            Items = new List<MenuItem>();
            //Settings = new Dictionary<string, string>();
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
