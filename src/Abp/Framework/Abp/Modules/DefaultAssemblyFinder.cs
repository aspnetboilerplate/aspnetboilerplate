using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abp.Modules
{
    /// <summary>
    /// Default assebly finder which looks assemblies in current domain.
    /// </summary>
    public class DefaultAssemblyFinder : IAssemblyFinder
    {
        /// <summary>
        /// Get all assemblies in current domain.
        /// </summary>
        /// <returns>list of assemblies</returns>
        public IEnumerable<Assembly> GetAllAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().ToList();
        }
    }
}
