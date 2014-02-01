using System;
using System.Collections.Generic;
using Abp.Application.Features;

namespace Abp.Application.Editions
{
    ///// <summary>
    ///// Represents an edition of the application.
    ///// </summary>
    //public class Edition
    //{
    //    /// <summary>
    //    /// Default edition.
    //    /// </summary>
    //    public static Edition Default { get; set; }

    //    /// <summary>
    //    /// Unique name of the edition.
    //    /// </summary>
    //    public string Name { get; private set; }

    //    /// <summary>
    //    /// Display name of this edition.
    //    /// </summary>
    //    public string DisplayName { get; set; }

    //    /// <summary>
    //    /// All features of this edition.
    //    /// </summary>
    //    public IDictionary<string, ApplicationFeature> Features { get; private set; }

    //    /// <summary>
    //    /// Gets the current edition.
    //    /// </summary>
    //    public static Edition Current
    //    {
    //        get { return _current ?? Default ?? NullEdition.Instance; }
    //        set { _current = value; }
    //    }
    //    private static Edition _current;

    //    /// <summary>
    //    /// Create a new Edition.
    //    /// </summary>
    //    /// <param name="name">Unique name of the edition.</param>
    //    public Edition(string name)
    //    {
    //        Name = name;
    //        DisplayName = name;
    //        Features = new Dictionary<string, ApplicationFeature>();
    //    }

    //    /// <summary>
    //    /// Gets a boolean value indicates that if this edition contains given feature.
    //    /// </summary>
    //    /// <param name="featureName">Name of the feature to check</param>
    //    public virtual bool HasFeature(string featureName)
    //    {
    //        return Features.ContainsKey(featureName);
    //    }
    //}
}
