using System.Collections.Generic;

namespace Abp.Application.Authorization.Permissions
{
    /// <summary>
    /// Permission manager.
    /// </summary>
    public interface IPermissionManager
    {
        /// <summary>
        /// Gets <see cref="PermissionDefinition"/> object with given <see cref="permissionName"/> or returns null
        /// if there is no permission with given <see cref="permissionName"/>.
        /// </summary>
        /// <param name="permissionName">Unique name of the permission</param>
        PermissionDefinition GetPermissionOrNull(string permissionName);

        /// <summary>
        /// Gets all permission definitions.
        /// </summary>
        IReadOnlyList<PermissionDefinition> GetAllPermissions();

        /// <summary>
        /// Gets root permission groups.
        /// All permission groups and permissions can be reached using this method.
        /// </summary>
        /// <returns>Root permission groups</returns>
        IReadOnlyList<PermissionGroup> GetRootPermissionGroups();
    }
}
