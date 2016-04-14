using Abp.Dependency;
using System;

namespace Abp.Application.Features
{
    /// <summary>
    /// Implementation of <see cref="IFeatureDependencyContext"/>.
    /// </summary>
    public class FeatureDependencyContext : IFeatureDependencyContext, ITransientDependency
    {
        public Guid? TenantId { get; set; }

        /// <inheritdoc/>
        public IIocResolver IocResolver { get; private set; }

        /// <inheritdoc/>
        public IFeatureChecker FeatureChecker { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureDependencyContext"/> class.
        /// </summary>
        public FeatureDependencyContext(IIocResolver iocResolver, IFeatureChecker featureChecker)
        {
            IocResolver = iocResolver;
            FeatureChecker = featureChecker;
        }
    }
}