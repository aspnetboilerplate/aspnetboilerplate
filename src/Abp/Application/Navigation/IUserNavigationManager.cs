using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Abp.Application.Navigation
{
    /// <summary>
    ///     Used to manage navigation for users.
    /// </summary>
    public interface IUserNavigationManager
    {
        /// <summary>
        ///     Gets a menu specialized for given user.
        /// </summary>
        /// <param name="menuName">Unique name of the menu</param>
        /// <param name="userId">User id or null for anonymous users</param>
        /// <param name="tenantId">Tenant id (optional). Uses current tenant if not provided.</param>
        [Obsolete("Use GetMenuAsync(UserIdentifier) instead.")]
        Task<UserMenu> GetMenuAsync(string menuName, Guid? userId, Guid? tenantId = null);

        /// <summary>
        ///     Gets a menu specialized for given user.
        /// </summary>
        /// <param name="menuName">Unique name of the menu</param>
        /// <param name="user">The user, or null for anonymous users</param>
        Task<UserMenu> GetMenuAsync(string menuName, UserIdentifier user);

        /// <summary>
        ///     Gets all menus specialized for given user.
        /// </summary>
        /// <param name="userId">User id or null for anonymous users</param>
        /// <param name="tenantId">Tenant id (optional). Uses current tenant if not provided.</param>
        [Obsolete("Use GetMenusAsync(UserIdentifier) instead.")]
        Task<IReadOnlyList<UserMenu>> GetMenusAsync(Guid? userId, Guid? tenantId = null);

        /// <summary>
        ///     Gets all menus specialized for given user.
        /// </summary>
        /// <param name="user">User id or null for anonymous users</param>
        Task<IReadOnlyList<UserMenu>> GetMenusAsync(UserIdentifier user);
    }
}