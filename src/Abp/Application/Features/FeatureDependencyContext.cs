using Abp.Dependency;

namespace Abp.Application.Features
{
    public class FeatureDependencyContext : IFeatureDependencyContext, ITransientDependency
    {
        public IIocResolver IocResolver { get; private set; }

        public IFeatureChecker FeatureChecker { get; private set; }

        public FeatureDependencyContext(IIocResolver iocResolver, IFeatureChecker featureChecker)
        {
            IocResolver = iocResolver;
            FeatureChecker = featureChecker;
        }
    }
}