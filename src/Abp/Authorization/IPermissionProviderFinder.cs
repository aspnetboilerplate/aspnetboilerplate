using System;
using System.Collections.Generic;

namespace Abp.Authorization
{
    /// <summary>
    /// This interface is used to find all classes implement <see cref="PermissionProvider"/> in the application.
    /// </summary>
    public interface IPermissionProviderFinder
    {
        /// <summary>
        /// Finds and returns all classes implement <see cref="PermissionProvider"/> in the application.
        /// </summary>
        List<Type> FindAll();
    }
}