namespace Abp.Authorization
{
    /// <summary>
    /// Defines interface to use authorization system.
    /// </summary>
    public interface IAuthorizationService //TODO: Move these methods to Permission manager and create a IPermissionStore!
    {
        /// <summary>
        /// Checks if current user is authorized for any of the given permissions.
        /// </summary>
        /// <param name="permissionNames">Name of the permissions</param>
        /// <returns>True: Yes, False: No.</returns>
        bool HasAnyOfPermissions(string[] permissionNames);

        /// <summary>
        /// Checks if current user is authorized for all of the given permissions.
        /// </summary>
        /// <param name="permissionNames">Name of the permissions</param>
        /// <returns>True: Yes, False: No.</returns>
        bool HasAllOfPermissions(string[] permissionNames);

        /// <summary>
        /// Gets all granted permission names for current user.
        /// </summary>
        /// <returns></returns>
        string[] GetGrantedPermissionNames();
    }
}