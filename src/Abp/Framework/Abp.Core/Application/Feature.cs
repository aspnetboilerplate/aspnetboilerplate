using System;
using System.Collections.Generic;

namespace Abp.Application
{
    /// <summary>
    /// Represents a feture of the application that can be authorized.
    /// </summary>
    public class Feature
    {
        //TODO: Think on multi-value feaures!

        /// <summary>
        /// Unique Name of the feature.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Display name of the feature.
        /// This can be used to show feature to the user.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// A brief description for this feature.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Creation date of this feature.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Parent feature.
        /// A feature can be a parent feature. If parent feature is
        /// disabled, this feature is disabled automatically.
        /// </summary>
        public Feature Parent { get; set; }

        /// <summary>
        /// Children features of this feature.
        /// A feature can be zero or more child features.
        /// If a feature is disabled, all children are disabled automatically.
        /// </summary>
        public List<Feature> Children { get; private set; }

        /// <summary>
        /// A list of dependent features for this feature.
        /// </summary>
        public List<Feature> Dependencies { get; private set; }

        /// <summary>
        /// Creates a new feature.
        /// </summary>
        /// <param name="name">Unique Name of the feature</param>
        public Feature(string name)
        {
            Name = name;
            DisplayName = "";
            Description = "";
            ReleaseDate = DateTime.MinValue;
            Children = new List<Feature>();
            Dependencies = new List<Feature>();
        }
    }
}