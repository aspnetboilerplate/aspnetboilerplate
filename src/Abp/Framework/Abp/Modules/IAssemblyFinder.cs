using System.Collections.Generic;
using System.Reflection;

namespace Abp.Modules
{
    /// <summary>
    /// Default assebly finder which looks assemblies in current domain.
    /// </summary>
    public interface IAssemblyFinder
    {
        /// <summary>
        /// Get all assemblies in current domain.
        /// </summary>
        /// <returns>list of assemblies</returns>
        IEnumerable<Assembly> GetAllAssemblies();
    }
}