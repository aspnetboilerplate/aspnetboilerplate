using System.Threading.Tasks;

namespace Abp.Application.Features
{
    /// <summary>
    /// Null pattern (default) implementation of <see cref="IFeatureValueStore"/>.
    /// It gets null for all feature values.
    /// <see cref="Instance"/> can be used via property injection of <see cref="IFeatureValueStore"/>.
    /// </summary>
    public class NullFeatureValueStore : IFeatureValueStore
    {
        /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static NullFeatureValueStore Instance { get; } = new NullFeatureValueStore();

        /// <inheritdoc/>
        public Task<string> GetValueOrNullAsync(long tenantId, Feature feature)
        {
            return Task.FromResult((string)null);
        }

        /// <inheritdoc/>
        public string GetValueOrNull(long tenantId, Feature feature)
        {
            return (string)null;
        }
    }
}
