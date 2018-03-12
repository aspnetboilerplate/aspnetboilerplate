using Abp.Dependency;

namespace Abp.Application.Features
{
    /// <summary>
    /// Used in the <see cref="IFeatureDependency.IsSatisfiedAsync"/> method.
    /// </summary>
    public interface IFeatureDependencyContext
    {
        /// <summary>
        /// Tenant id which requires the feature.
        /// Null for current tenant.
        /// </summary>
        int? TenantId { get; }

        /// <summary>
        /// Gets the <see cref="IIocResolver"/>.
        /// </summary>
        /// <value>
        /// The ioc resolver.
        /// </value>
        IIocResolver IocResolver { get; }

        /// <summary>
        /// Gets the <see cref="IFeatureChecker"/>.
        /// </summary>
        /// <value>
        /// The feature checker.
        /// </value>
        IFeatureChecker FeatureChecker { get; }
    }
}