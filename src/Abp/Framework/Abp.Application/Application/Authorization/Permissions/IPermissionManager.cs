using Abp.Dependency;

namespace Abp.Application.Authorization.Permissions
{
    /// <summary>
    /// Permission manager.
    /// </summary>
    public interface IPermissionManager : ISingletonDependency
    {
        /// <summary>
        /// Gets <see cref="Permission"/> object with given <see cref="permissionName"/> or returns null
        /// if there is no permission with given <see cref="permissionName"/>.
        /// </summary>
        /// <param name="permissionName">Unique name of the permission</param>
        Permission GetPermissionOrNull(string permissionName);
    }
}
