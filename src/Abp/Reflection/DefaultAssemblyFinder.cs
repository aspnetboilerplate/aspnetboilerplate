using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Abp.Dependency;

namespace Abp.Reflection
{

    /// <summary>
    ///     Default implementation of <see cref="IAssemblyFinder" />.
    ///     If gets assemblies from current domain.
    /// </summary>
    internal class DefaultAssemblyFinder : IAssemblyFinder
    {
        #region Private Members

        /// <summary>
        ///     Private constructor to disable instancing.
        /// </summary>
        private DefaultAssemblyFinder(IAssemblyFilter aAssemblyFilter)
        {
            _DefaultAssemblyFilter = aAssemblyFilter;
        }

        private static readonly DefaultAssemblyFinder _SingletonInstance = new DefaultAssemblyFinder(IocManager.Instance.Resolve<IAssemblyFilter>());

        private readonly IAssemblyFilter _DefaultAssemblyFilter;

        #endregion

        #region Public Members

        /// <summary>
        /// Gets the assemblies within the application domain.
        /// </summary>
        /// <returns>Returns the list of assemblies filtered by IAssemblyFilter should any need to be excluded.</returns>
        public List<Assembly> GetAllAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                            .Where(asm => !_DefaultAssemblyFilter.ExcludeAssembly(asm.FullName))
                            .ToList();
        }

        /// <summary>
        ///     Gets Singleton instance of <see cref="DefaultAssemblyFinder" />.
        /// </summary>
        public static DefaultAssemblyFinder Instance
        {
            get { return _SingletonInstance; }
        }

        #endregion
    }
}