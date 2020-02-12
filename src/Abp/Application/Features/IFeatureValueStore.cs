using System.Threading.Tasks;

namespace Abp.Application.Features
{
    /// <summary>
    /// Defines a store to get a feature's value.
    /// </summary>
    public interface IFeatureValueStore
    {
        /// <summary>
        /// Gets the feature value or null.
        /// </summary>
        /// <param name="tenantId">The tenant id.</param>
        /// <param name="feature">The feature.</param>
        Task<string> GetValueOrNullAsync(int tenantId, Feature feature);

        /// <summary>
        /// Gets the feature value or null.
        /// </summary>
        /// <param name="tenantId">The tenant id.</param>
        /// <param name="feature">The feature.</param>
        string GetValueOrNull(int tenantId, Feature feature);
    }
}