using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Collections.Extensions;
using Abp.Localization;

namespace Abp.Application.Features
{
    /// <summary>
    /// Defines a feature of the application.
    /// </summary>
    public class Feature
    {
        /// <summary>
        /// Gets/sets arbitrary objects related to this object.
        /// Gets null if given key does not exists.
        /// </summary>
        /// <param name="key">Key</param>
        public object this[string key]
        {
            get { return Attributes.GetOrDefault(key); }
            set { Attributes[key] = value; }
        }

        /// <summary>
        /// Arbitrary objects related to this object.
        /// </summary>
        public IDictionary<string, object> Attributes { get; set; }

        /// <summary>
        /// Parent of this feature, if one exists.
        /// If set, this feature can be enabled only if parent is enabled.
        /// </summary>
        public Feature Parent { get; private set; }

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
        /// Feature scope.
        /// </summary>
        public FeatureScopes Scope { get; set; }

        /// <summary>
        /// List of child features. A child feature can be enabled only if parent is enabled.
        /// </summary>
        public IReadOnlyList<Feature> Children
        {
            get { return _children.ToImmutableList(); }
        }
        private readonly List<Feature> _children;

        /// <summary>
        /// Creates a new feature.
        /// </summary>
        /// <param name="name">Unique name of the feature</param>
        /// <param name="isDisabled">This property can be used to disable this feature completely.</param>
        /// <param name="displayName">Display name of the feature</param>
        /// <param name="description">A brief description for this feature</param>
        /// <param name="scope">Feature scope</param>
        public Feature(string name, ILocalizableString displayName = null, bool isDisabled = false, ILocalizableString description = null, FeatureScopes scope = FeatureScopes.All)
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

            _children = new List<Feature>();
        }

        /// <summary>
        /// Adds a child feature.
        /// A child feature can be enabled only if parent is enabled.
        /// </summary>
        /// <returns>Returns newly created child feature</returns>
        public Feature CreateChildFeature(string name, ILocalizableString displayName = null, bool isDisabled = false, ILocalizableString description = null, FeatureScopes scope = FeatureScopes.All)
        {
            var feature = new Feature(name, displayName, isDisabled, description, scope) { Parent = this };
            _children.Add(feature);
            return feature;
        }

        public override string ToString()
        {
            return string.Format("[Feature: {0}]", Name);
        }
    }
}
