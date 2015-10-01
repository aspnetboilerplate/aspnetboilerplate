using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Localization;

namespace Abp.Application.Features
{
    /// <summary>
    /// Defines a feature of the application.
    /// </summary>
    public class ApplicationFeature
    {
        /// <summary>
        /// Parent of this feature, if one exists.
        /// If set, this feature can be enabled only if parent is enabled.
        /// </summary>
        public ApplicationFeature Parent { get; private set; }

        /// <summary>
        /// Unique name of the feature.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Display name of the feature.
        /// This can be used to show features on UI.
        /// </summary>
        public ILocalizableString DisplayName { get; set; }

        /// <summary>
        /// A brief description for this feature.
        /// This can be used to show feature description on UI. 
        /// </summary>
        public ILocalizableString Description { get; set; }

        /// <summary>
        /// This property can be used to disable this feature completely.
        /// Default value: false.
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Feature scope
        /// </summary>
        public ApplicationFeatureScopes Scope { get; set; }

        /// <summary>
        /// List of child features. A child feature can be enabled only if parent is enabled.
        /// </summary>
        public IReadOnlyList<ApplicationFeature> Children
        {
            get { return _children.ToImmutableList(); }
        }
        private readonly List<ApplicationFeature> _children;

        /// <summary>
        /// Creates a new feature.
        /// </summary>
        /// <param name="name">Unique name of the feature</param>
        /// <param name="isDisabled">This property can be used to disable this feature completely.</param>
        /// <param name="displayName">Display name of the feature</param>
        /// <param name="description">A brief description for this feature</param>
        public ApplicationFeature(string name, ILocalizableString displayName = null, bool isDisabled = false, ILocalizableString description = null, ApplicationFeatureScopes scope = ApplicationFeatureScopes.All)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            Name = name;
            DisplayName = displayName;
            IsDisabled = isDisabled;
            Description = description;
            Scope = scope;

            _children = new List<ApplicationFeature>();
        }

        /// <summary>
        /// Adds a child feature.
        /// A child feature can be enabled only if parent is enabled.
        /// </summary>
        /// <returns>Returns newly created child feature</returns>
        public ApplicationFeature CreateChildFeature(string name, ILocalizableString displayName = null, bool isDisabled = false, ILocalizableString description = null, ApplicationFeatureScopes scope = ApplicationFeatureScopes.All)
        {
            var feature = new ApplicationFeature(name, displayName, isDisabled, description, scope) { Parent = this };
            _children.Add(feature);
            return feature;
        }

        public override string ToString()
        {
            return string.Format("[Feature: {0}]", Name);
        }
    }
}
