using System.Collections.Generic;

namespace Adorable.Application.Navigation
{
    /// <summary>
    /// Manages navigation in the application.
    /// </summary>
    public interface INavigationManager
    {
        /// <summary>
        /// All menus defined in the application.
        /// </summary>
        IDictionary<string, MenuDefinition> Menus { get; }

        /// <summary>
        /// Gets the main menu of the application.
        /// A shortcut of <see cref="Menus"/>["MainMenu"].
        /// </summary>
        MenuDefinition MainMenu { get; }
    }
}
