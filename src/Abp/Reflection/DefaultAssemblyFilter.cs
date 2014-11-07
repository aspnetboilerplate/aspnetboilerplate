namespace Abp.Reflection
{
    /// <summary>
    ///     Default implementation of <see cref="IAssemblyFilter" />.
    ///     It provides a mechanism to filter any assemblies found
    /// </summary>
    internal class DefaultAssemblyFilter : IAssemblyFilter
    {
        #region Private Members

        /// <summary>
        ///     Private constructor to disable instancing.
        /// </summary>
        private DefaultAssemblyFilter()
        {
        }

        private static readonly DefaultAssemblyFilter _SingletonInstance = new DefaultAssemblyFilter();

        #endregion

        #region Implementation of IAssemblyFilter

        /// <summary>
        ///     Allows assemblies found by IAssemblyFinder to be excluded as needed.
        /// </summary>
        /// <param name="aAssemblyFullName">Full name of the assembly</param>
        /// <returns>Return TRUE to exclude the assembly in question, FALSE otherwise</returns>
        public bool ExcludeAssembly(string aAssemblyFullName)
        {
            //By default we exclude nothing
            return false;
        }

        #endregion

        #region Public Members

        /// <summary>
        ///     Gets Singleton instance of <see cref="DefaultAssemblyFilter" />.
        /// </summary>
        public static DefaultAssemblyFilter Instance
        {
            get { return _SingletonInstance; }
        }

        #endregion
    }
}