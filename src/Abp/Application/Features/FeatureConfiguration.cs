using Abp.Collections;

namespace Abp.Application.Features
{
    internal class FeatureConfiguration : IFeatureConfiguration
    {
        public FeatureConfiguration()
        {
            Providers = new TypeList<FeatureProvider>();
        }

        public ITypeList<FeatureProvider> Providers { get; }
    }
}