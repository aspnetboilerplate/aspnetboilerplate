using System.Collections.Generic;

namespace Abp.Application.Authorization.Permissions
{
    /// <summary>
    /// This interface defines a class that provide a list of permissions those are used for authorization in the application.
    /// </summary>
    public interface IPermissionDefinitionProvider
    {
        /// <summary>
        /// Gets a list of <see cref="PermissionDefinition"/> objects.
        /// </summary>
        /// <returns>Permissions</returns>
        IEnumerable<PermissionDefinition> GetPermissions(PermissionDefinitionProviderContext context);
    }
}