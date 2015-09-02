using System;
using System.Collections.Generic;

namespace Abp.Modules
{
    /// <summary>
    /// This interface is responsible to find all modules (derived from <see cref="AbpModule"/>)
    /// in the application.
    /// </summary>
    public interface IModuleFinder
    {
        /// <summary>
        /// Finds all modules.
        /// </summary>
        /// <returns>List of modules</returns>
        ICollection<Type> FindAll();
    }
}