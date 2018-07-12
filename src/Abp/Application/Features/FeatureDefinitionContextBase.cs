using Abp.Collections.Extensions;
using Abp.Localization;
using Abp.UI.Inputs;

namespace Abp.Application.Features
{
    /// <summary>
    /// Base for implementing <see cref="IFeatureDefinitionContext"/>.
    /// </summary>
    public abstract class FeatureDefinitionContextBase : IFeatureDefinitionContext
    {
        protected readonly FeatureDictionary Features;

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureDefinitionContextBase"/> class.
        /// </summary>
        protected FeatureDefinitionContextBase()
        {
            Features = new FeatureDictionary();
        }

        /// <summary>
        /// Creates a new feature.
        /// </summary>
        /// <param name="name">Unique name of the feature</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="displayName">Display name of the feature</param>
        /// <param name="description">A brief description for this feature</param>
        /// <param name="scope">Feature scope</param>
        /// <param name="inputType">Input type</param>
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

        /// <summary>
        /// Gets a feature with a given name, or null if can not be found.
        /// </summary>
        /// <param name="name">Unique name of the feature</param>
        /// <returns>
        ///   <see cref="Feature" /> object or null
        /// </returns>
        public Feature GetOrNull(string name)
        {
            return Features.GetOrDefault(name);
        }

        /// <summary>
        /// Remove feature with given name
        /// </summary>
        /// <param name="name"></param>
        public void Remove(string name)
        {
            Features.Remove(name);
        }
    }
}