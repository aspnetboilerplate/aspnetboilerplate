using Abp.Dependency;

namespace Abp.Application.Features
{
    public interface IFeatureDependencyContext
    {
        IIocResolver IocResolver { get; }

        IFeatureChecker FeatureChecker { get; }
    }
}