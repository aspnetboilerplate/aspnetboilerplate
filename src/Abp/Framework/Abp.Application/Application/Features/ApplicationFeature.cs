using System.Collections.Generic;

namespace Abp.Application.Features
{
    ///// <summary>
    ///// Represents a feture of the application that can be authorized.
    ///// </summary>
    //public class ApplicationFeature
    //{
    //    /// <summary>
    //    /// Unique Name of the feature.
    //    /// </summary>
    //    public string Name { get; private set; }

    //    /// <summary>
    //    /// Display name of the feature.
    //    /// This can be used to show feature to the user.
    //    /// </summary>
    //    public string DisplayName { get; set; }

    //    /// <summary>
    //    /// A brief description for this feature.
    //    /// </summary>
    //    public string Description { get; set; }

    //    /// <summary>
    //    /// Parent feature.
    //    /// A feature can has a parent feature. If parent feature is
    //    /// disabled, this feature is disabled automatically.
    //    /// </summary>
    //    public ApplicationFeature Parent { get; set; }

    //    /// <summary>
    //    /// Children features of this feature.
    //    /// A feature can be zero or more child features.
    //    /// If a feature is disabled, all children are disabled automatically.
    //    /// </summary>
    //    public List<ApplicationFeature> Children { get; private set; }

    //    /// <summary>
    //    /// A list of dependent features for this feature.
    //    /// A feature can be enabled only if all dependencies are enabled.
    //    /// </summary>
    //    public List<ApplicationFeature> Dependencies { get; private set; }

    //    /// <summary>
    //    /// Creates a new feature.
    //    /// </summary>
    //    /// <param name="name">Unique Name of the feature</param>
    //    public ApplicationFeature(string name)
    //    {
    //        Name = name;
    //        DisplayName = Name;
    //        Description = Name;
    //        Children = new List<ApplicationFeature>();
    //        Dependencies = new List<ApplicationFeature>();
    //    }
    //}
}