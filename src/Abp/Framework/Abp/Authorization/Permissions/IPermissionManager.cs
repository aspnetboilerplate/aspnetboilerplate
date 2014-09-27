using System.Collections.Generic;

namespace Abp.Authorization.Permissions
{
    /// <summary>
    /// Permission manager.
    /// </summary>
    public interface IPermissionManager
    {
        /// <summary>
        /// Gets <see cref="Permission"/> object with given <see cref="name"/> or returns null
        /// if there is no permission with given <see cref="name"/>.
        /// </summary>
        /// <param name="name">Unique name of the permission</param>
        Permission GetPermissionOrNull(string name);

        /// <summary>
        /// Gets all permissions.
        /// </summary>
        IReadOnlyList<Permission> GetAllPermissions();

        /// <summary>
        /// Gets all root permission groups.
        /// All permission groups and permissions can be reached using this method.
        /// </summary>
        /// <returns>Root permission groups</returns>
        IReadOnlyList<PermissionGroup> GetAllRootGroups();

        /// <summary>
        /// Gets <see cref="PermissionGroup"/> object with given <see cref="name"/> or returns null
        /// if there is no permission group with given <see cref="name"/>.
        /// </summary>
        /// <param name="name">Unique name of the permission</param>
        PermissionGroup GetRootGroupOrNull(string name);
    }
}
