using Abp.Collections.Extensions;
using Abp.Localization;
using Abp.UI.Inputs;

namespace Abp.Application.Features
{
    public class FeatureDefinitionContextBase : IFeatureDefinitionContext
    {
        protected readonly FeatureDictionary Features;

        public FeatureDefinitionContextBase()
        {
            Features = new FeatureDictionary();
        }

        public Feature Create(string name, string defaultValue, ILocalizableString displayName = null,
            ILocalizableString description = null, FeatureScopes scope = FeatureScopes.All, IInputType inputType = null)
        {
            if (Features.ContainsKey(name))
            {
                throw new AbpException("There is already a feature with name: " + name);
            }

            var feature = new Feature(name, defaultValue, displayName, description, scope, inputType);
            Features[feature.Name] = feature;
            return feature;

        }

        public Feature GetOrNull(string name)
        {
            return Features.GetOrDefault(name);
        }
    }
}