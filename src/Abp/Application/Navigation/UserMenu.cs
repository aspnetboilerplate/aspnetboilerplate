using System.Collections.Generic;

namespace Abp.Application.Navigation
{
    /// <summary>
    /// Represents a menu shown to the user.
    /// </summary>
    public class UserMenu
    {
        /// <summary>
        /// Unique name of the menu in the application. 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Display name of the menu.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Menu items (first level).
        /// </summary>
        public IList<UserMenuItem> Items { get; set; }

        /// <summary>
        /// Creates a new <see cref="UserMenu"/> object.
        /// </summary>
        public UserMenu()
        {
            
        }

        /// <summary>
        /// Creates a new <see cref="UserMenu"/> object from given <see cref="MenuDefinition"/>.
        /// </summary>
        public UserMenu(MenuDefinition menuDefinition)
        {
            Name = menuDefinition.Name;
            DisplayName = menuDefinition.DisplayName.Localize();
            Items = new List<UserMenuItem>();
        }
    }
}