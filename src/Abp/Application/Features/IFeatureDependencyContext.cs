using Abp.Dependency;

namespace Abp.Application.Features
{
    public interface IFeatureDependencyContext
    {
        IIocResolver IocResolver { get; }

        IFeatureChecker FeatureChecker { get; }
    }

    public class FeatureDependencyContext : IFeatureDependencyContext
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