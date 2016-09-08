using Abp.Dependency;

namespace Abp.Application.Features
{
    /// <summary>
    /// This class should be inherited in order to provide <see cref="Feature"/>s.
    /// </summary>
    public abstract class FeatureProvider : ITransientDependency
    {
        /// <summary>
        /// Used to set <see cref="Feature"/>s.
        /// </summary>
        /// <param name="context">Feature definition context</param>
        public abstract void SetFeatures(IFeatureDefinitionContext context);
    }
}