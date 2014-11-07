namespace Abp.Reflection
{
    /// <summary>
    ///     Used by IAssemblyFinder to potentially exclude assemblies found.
    /// </summary>
    public interface IAssemblyFilter
    {
        #region Public Members

        /// <summary>
        ///     Allows assemblies found by IAssemblyFinder to be excluded as needed.
        /// </summary>
        /// <param name="aAssemblyFullName">Full name of the assembly</param>
        /// <returns>Return TRUE to exclude the assembly in question, FALSE otherwise</returns>
        bool ExcludeAssembly(string aAssemblyFullName);

        #endregion
    }
}