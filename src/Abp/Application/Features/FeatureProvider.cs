namespace Abp.Application.Features
{
    public abstract class FeatureProvider
    {
        public abstract void SetFeatures(IFeatureDefinitionContext context);
    }
}