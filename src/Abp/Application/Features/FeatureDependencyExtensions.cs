using Abp.Threading;

namespace Abp.Application.Features
{
    public static class FeatureDependencyExtensions
    {
        public static bool IsSatisfied(this IFeatureDependency featureDependency, IFeatureDependencyContext context)
        {
            return AsyncHelper.RunSync(() => featureDependency.IsSatisfiedAsync(context));
        }
    }
}