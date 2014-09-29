namespace Abp.Dependency
{
    /// <summary>
    /// This interface is used to register dependencies by conventions. 
    /// </summary>
    /// <remarks>
    /// Implement this interface and register to <see cref="IocManager.AddConventionalRegistrar"/> method to be able
    /// to register classes by your own conventions.
    /// </remarks>
    public interface IConventionalDependencyRegistrar
    {
        /// <summary>
        /// Registers types of given assembly by convention.
        /// </summary>
        /// <param name="context">Registration context</param>
        void RegisterAssembly(IConventionalRegistrationContext context);
    }
}