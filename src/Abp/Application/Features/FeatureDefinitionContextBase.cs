using Abp.Collections.Extensions;
using Abp.Localization;

namespace Abp.Application.Features
{
    public class FeatureDefinitionContextBase : IFeatureDefinitionContext
    {
        protected readonly FeatureDictionary Features;

        public FeatureDefinitionContextBase()
        {
            Features = new FeatureDictionary();
        }

        public Feature Create(string name, ILocalizableString displayName = null, bool isDisabled = false,
            ILocalizableString description = null, FeatureScopes scope = FeatureScopes.All)
        {
            if (Features.ContainsKey(name))
            {
                throw new AbpException("There is already a feature with name: " + name);
            }

            var feature = new Feature(name, displayName, isDisabled, description, scope);
            Features[feature.Name] = feature;
            return feature;

        }

        public Feature GetOrNull(string name)
        {
            return Features.GetOrDefault(name);
        }
    }
}