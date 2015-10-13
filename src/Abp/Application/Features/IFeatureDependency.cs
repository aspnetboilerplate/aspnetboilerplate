using System.Threading.Tasks;

namespace Abp.Application.Features
{
    public interface IFeatureDependency
    {
        /// <summary>
        /// Implementation should check depended features and returns true if dependencies are satisfied.
        /// </summary>
        Task<bool> IsSatisfiedAsync(IFeatureDependencyContext context);
    }
}