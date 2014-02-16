using Abp.Dependency;

namespace Abp.Application.Authorization
{
    /// <summary>
    /// Defines interface to use authorization system.
    /// </summary>
    public interface IAuthorizationService : ITransientDependency
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
    }
}