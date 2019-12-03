using System.Threading.Tasks;

namespace Abp.Application.Features
{
    /// <summary>
    /// Defines a feature dependency.
    /// </summary>
    public interface IFeatureDependency
    {
        /// <summary>
        /// Checks dependent features and returns true if the dependencies are satisfied.
        /// </summary>
        Task<bool> IsSatisfiedAsync(IFeatureDependencyContext context);

        /// <summary>
        /// Checks dependent features and returns true if the dependencies are satisfied.
        /// </summary>
        bool IsSatisfied(IFeatureDependencyContext context);
    }
}