using System.Collections.Generic;

namespace Abp.Application.Navigation
{
    /// <summary>
    /// Used to manage navigation for users.
    /// </summary>
    public interface IUserNavigationManager
    {
        /// <summary>
        /// Gets a menu specialized for given user.
        /// </summary>
        /// <param name="menuName">Unique name of the menu</param>
        /// <param name="userId">User id</param>
        UserMenu GetMenu(string menuName, long userId);

        /// <summary>
        /// Gets all menus specialized for given user.
        /// </summary>
        /// <param name="userId">User id</param>
        IReadOnlyList<UserMenu> GetMenus(long userId);
    }
}