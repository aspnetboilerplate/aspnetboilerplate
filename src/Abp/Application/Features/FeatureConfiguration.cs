using Abp.Collections;

namespace Abp.Application.Features
{
    /// <summary>
    /// Internal implementation for <see cref="IFeatureConfiguration"/>.
    /// </summary>
    internal class FeatureConfiguration : IFeatureConfiguration
    {
        /// <summary>
        /// Reference to the feature providers.
        /// </summary>
        public ITypeList<FeatureProvider> Providers { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureConfiguration"/> class.
        /// </summary>
        public FeatureConfiguration()
        {
            Providers = new TypeList<FeatureProvider>();
        }
    }
}