using System.Collections.Generic;

namespace Abp.Application.Authorization.Permissions
{
    /// <summary>
    /// This interface is used to find all classes implements <see cref="IPermissionDefinitionProvider"/> in the application.
    /// </summary>
    public interface IPermissionDefinitionProviderFinder
    {
        /// <summary>
        /// Finds and returns all classes implements <see cref="IPermissionDefinitionProvider"/> in the application.
        /// </summary>
        IEnumerable<IPermissionDefinitionProvider> GetPermissionProviders();
    }
}