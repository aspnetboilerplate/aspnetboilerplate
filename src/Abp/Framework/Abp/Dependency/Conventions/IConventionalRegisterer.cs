using System.Reflection;
using Castle.Windsor;

namespace Abp.Dependency.Conventions
{
    /// <summary>
    /// This interface is used to register dependencies by conventions. 
    /// </summary>
    public interface IConventionalRegisterer
    {
        /// <summary>
        /// Registers types of given assembly by convention.
        /// </summary>
        /// <param name="container">Dependency container</param>
        /// <param name="assembly">Assembly to register</param>
        void RegisterAssembly(IWindsorContainer container, Assembly assembly);
    }
}