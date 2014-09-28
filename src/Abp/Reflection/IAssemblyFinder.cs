using System.Collections.Generic;
using System.Reflection;

namespace Abp.Reflection
{
    /// <summary>
    /// This interface is used to get all assemblies to investigate special classes
    /// such as ABP modules.
    /// </summary>
    public interface IAssemblyFinder
    {
        /// <summary>
        /// This method should return all assemblies used by application.
        /// </summary>
        /// <returns>List of assemblies</returns>
        List<Assembly> GetAllAssemblies();
    }
}