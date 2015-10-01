using Abp.Localization;

namespace Abp.Application.Features
{
    public interface IFeatureDefinitionContext
    {
        /// <summary>
        /// Creates a new feature.
        /// </summary>
        /// <param name="name">Unique name of the feature</param>
        /// <param name="isDisabled">This property can be used to disable this feature completely.</param>
        /// <param name="displayName">Display name of the feature</param>
        /// <param name="description">A brief description for this feature</param>
        /// <param name="scope">Feature scope</param>
        Feature Create(string name, ILocalizableString displayName = null, bool isDisabled = false, ILocalizableString description = null, FeatureScopes scope = FeatureScopes.All);

        /// <summary>
        /// Gets a feature with given name or null if can not find.
        /// </summary>
        /// <param name="name">Unique name of the feature</param>
        /// <returns><see cref="Feature"/> object or null</returns>
        Feature GetOrNull(string name);
    }
}