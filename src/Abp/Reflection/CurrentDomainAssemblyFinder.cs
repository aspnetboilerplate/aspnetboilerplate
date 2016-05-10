using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abp.Reflection
{
    /// <summary>
    ///     Default implementation of <see cref="IAssemblyFinder" />.
    ///     If gets assemblies from current domain.
    /// </summary>
    public class CurrentDomainAssemblyFinder : IAssemblyFinder
    {
        /// <summary>
        ///     Gets Singleton instance of <see cref="CurrentDomainAssemblyFinder" />.
        /// </summary>
        public static CurrentDomainAssemblyFinder Instance { get; } = new CurrentDomainAssemblyFinder();

        public List<Assembly> GetAllAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().ToList();
        }
    }
}