using System.Threading.Tasks;

namespace Abp.Authorization
{
    /// <summary>
    /// Defines interface to check a dependency.
    /// </summary>
    public interface IPermissionDependency
    {
        /// <summary>
        /// Checks if permission dependency is satisfied.
        /// </summary>
        /// <param name="context">Context.</param>
        Task<bool> IsSatisfiedAsync(IPermissionDependencyContext context);

        /// <summary>
        /// Checks if permission dependency is satisfied.
        /// </summary>
        /// <param name="context">Context.</param>
        bool IsSatisfied(IPermissionDependencyContext context);
    }
}