using System.Collections.Generic;

namespace Abp.Authorization
{
    /// <summary>
    /// Permission manager.
    /// </summary>
    public interface IPermissionManager
    {
        /// <summary>
		/// Gets <see cref="Permission"/> object with given <paramref name="name"/> or returns null
		/// if there is no permission with given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Unique name of the permission</param>
        Permission GetPermissionOrNull(string name);

        /// <summary>
        /// Gets all permissions.
        /// </summary>
        IReadOnlyList<Permission> GetAllPermissions();

        /// <summary>
		/// Gets <see cref="PermissionGroup"/> object with given <paramref name="name"/> or returns null
		/// if there is no permission group with given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">Unique name of the permission</param>
        PermissionGroup GetRootGroupOrNull(string name);

        /// <summary>
        /// Gets all root permission groups.
        /// All permission groups and permissions can be reached using this method.
        /// </summary>
        /// <returns>Root permission groups</returns>
        IReadOnlyList<PermissionGroup> GetAllRootGroups();

        /// <summary>
        /// Checks if a user is granted for a permission.
        /// </summary>
        /// <param name="userId">Id of the user to check</param>
        /// <param name="permissionName">Name of the permission</param>
        bool IsGranted(long userId, string permissionName);

        /// <summary>
        /// Gets all granted permissions for current user.
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<Permission> GetGrantedPermissions(long userId);
    }
}
