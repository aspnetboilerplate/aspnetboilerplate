using System.Threading.Tasks;

namespace Abp.Application.Features
{
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

        public SimpleFeatureDependency(params string[] features)
        {
            Features = features;
        }

        public SimpleFeatureDependency(bool requiresAll, params string[] features)
            : this(features)
        {
            RequiresAll = requiresAll;
        }

        public Task<bool> IsSatisfiedAsync(IFeatureDependencyContext context)
        {
            return context.FeatureChecker.IsEnabledAsync(RequiresAll, Features);
        }
    }
}