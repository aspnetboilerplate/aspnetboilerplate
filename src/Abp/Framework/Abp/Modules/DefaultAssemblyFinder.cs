using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Abp.Modules
{
    /// <summary>
    /// Default implementation of <see cref="IAssemblyFinder"/>.
    /// If gets assemblies from current domain.
    /// </summary>
    public class DefaultAssemblyFinder : IAssemblyFinder
    {
        /// <summary>
        /// Gets Singleton instance of <see cref="DefaultAssemblyFinder"/>.
        /// </summary>
        public static DefaultAssemblyFinder Instance { get { return SingletonInstance; } }
        private static readonly DefaultAssemblyFinder SingletonInstance = new DefaultAssemblyFinder();

        /// <summary>
        /// Private constructor to disable instancing.
        /// </summary>
        private DefaultAssemblyFinder()
        {

        }

        public List<Assembly> GetAllAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies().ToList();
        }
    }
}