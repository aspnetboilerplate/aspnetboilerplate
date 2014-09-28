using System;
using System.Collections.Generic;

namespace Abp.Authorization.Permissions
{
    /// <summary>
    /// This interface is used to find all classes implement <see cref="IPermissionProvider"/> in the application.
    /// </summary>
    public interface IPermissionProviderFinder
    {
        /// <summary>
        /// Finds and returns all classes implement <see cref="IPermissionProvider"/> in the application.
        /// </summary>
        List<Type> FindAll();
    }
}