using System.Collections.Generic;

namespace Abp.Application.Navigation
{
    /// <summary>
    /// Manages navigation in the application.
    /// </summary>
    public interface INavigationManager
    {
        /// <summary>
        /// All menus defined in the application.
        /// </summary>
        IDictionary<string, Menu> Menus { get; }

        /// <summary>
        /// Gets the main menu of the application.
        /// A shortcut of <see cref="Menus"/>["MainMenu"].
        /// </summary>
        Menu MainMenu { get; }
    }
}
