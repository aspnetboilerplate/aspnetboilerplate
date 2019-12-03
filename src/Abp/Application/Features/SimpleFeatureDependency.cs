using System.Threading.Tasks;

namespace Abp.Application.Features
{
    /// <summary>
    /// Most simple implementation of <see cref="IFeatureDependency"/>.
    /// It checks one or more features if they are enabled.
    /// </summary>
    public class SimpleFeatureDependency : IFeatureDependency
    {
        /// <summary>
        /// A list of features to be checked if they are enabled.
        /// </summary>
        public string[] Features { get; set; }

        /// <summary>
        /// If this property is set to true, all of the <see cref="Features"/> must be enabled.
        /// If it's false, at least one of the <see cref="Features"/> must be enabled.
        /// Default: false.
        /// </summary>
        public bool RequiresAll { get; set; }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleFeatureDependency"/> class.
        /// </summary>
        /// <param name="features">The features.</param>
        public SimpleFeatureDependency(params string[] features)
        {
            Features = features;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleFeatureDependency"/> class.
        /// </summary>
        /// <param name="requiresAll">
        /// If this is set to true, all of the <see cref="Features"/> must be enabled.
        /// If it's false, at least one of the <see cref="Features"/> must be enabled.
        /// </param>
        /// <param name="features">The features.</param>
        public SimpleFeatureDependency(bool requiresAll, params string[] features)
            : this(features)
        {
            RequiresAll = requiresAll;
        }

        /// <inheritdoc/>
        public Task<bool> IsSatisfiedAsync(IFeatureDependencyContext context)
        {
            return context.TenantId.HasValue
                ? context.FeatureChecker.IsEnabledAsync(context.TenantId.Value, RequiresAll, Features)
                : context.FeatureChecker.IsEnabledAsync(RequiresAll, Features);
        }

        /// <inheritdoc/>
        public bool IsSatisfied(IFeatureDependencyContext context)
        {
            return context.TenantId.HasValue
                ? context.FeatureChecker.IsEnabled(context.TenantId.Value, RequiresAll, Features)
                : context.FeatureChecker.IsEnabled(RequiresAll, Features);
        }
    }
}