using Abp.Localization;
using Abp.UI.Inputs;

namespace Abp.Application.Features
{
    /// <summary>
    /// Used in <see cref="FeatureProvider.SetFeatures"/> method as context.
    /// </summary>
    public interface IFeatureDefinitionContext
    {
        /// <summary>
        /// Creates a new feature.
        /// </summary>
        /// <param name="name">Unique name of the feature</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="displayName">Display name of the feature</param>
        /// <param name="description">A brief description for this feature</param>
        /// <param name="scope">Feature scope</param>
        /// <param name="inputType">Input type</param>
        Feature Create(string name, string defaultValue, ILocalizableString displayName = null, ILocalizableString description = null, FeatureScopes scope = FeatureScopes.All, IInputType inputType = null);

        /// <summary>
        /// Gets a feature with given name or null if can not find.
        /// </summary>
        /// <param name="name">Unique name of the feature</param>
        /// <returns><see cref="Feature"/> object or null</returns>
        Feature GetOrNull(string name);
    }
}