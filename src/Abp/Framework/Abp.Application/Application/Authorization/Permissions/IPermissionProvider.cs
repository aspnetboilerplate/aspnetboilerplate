using System.Collections.Generic;

namespace Abp.Application.Authorization.Permissions
{
    /// <summary>
    /// This interface defines a class that provide a list of permissions those are used for authorization in the application.
    /// </summary>
    public interface IPermissionProvider
    {
        /// <summary>
        /// Gets a list of <see cref="Permission"/> objects.
        /// </summary>
        /// <returns>Permissions</returns>
        IEnumerable<Permission> GetPermissions();
    }
}