using System;
using System.Collections.Generic;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Localization;

namespace Abp.Notifications
{
    /// <summary>
    /// Definition for a notification.
    /// </summary>
    public class NotificationDefinition
    {
        /// <summary>
        /// Unique name of the notification.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Related entity type with this notification (optional).
        /// </summary>
        public Type EntityType { get; private set; }

        /// <summary>
        /// Display name of the notification.
        /// Optional.
        /// </summary>
        public ILocalizableString DisplayName { get; set; }

        /// <summary>
        /// Description for the notification.
        /// Optional.
        /// </summary>
        public ILocalizableString Description { get; set; }

        /// <summary>
        /// A permission dependency. This notification will be available to a user if this dependency is satisfied.
        /// Optional.
        /// </summary>
        public IPermissionDependency PermissionDependency { get; set; }
        
        /// <summary>
        /// A feature dependency. This notification will be available to a tenant if this feature is enabled.
        /// Optional.
        /// </summary>
        public IFeatureDependency FeatureDependency { get; set; }

        /// <summary>
        /// Gets/sets arbitrary objects related to this object.
        /// Gets null if given key does not exists.
        /// This is a shortcut for <see cref="Attributes"/> dictionary.
        /// </summary>
        /// <param name="key">Key</param>
        public object this[string key]
        {
            get { return Attributes.GetOrDefault(key); }
            set { Attributes[key] = value; }
        }

        /// <summary>
        /// Arbitrary objects related to this object.
        /// These objects must be serializable.
        /// </summary>
        public IDictionary<string, object> Attributes { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationDefinition"/> class.
        /// </summary>
        /// <param name="name">Unique name of the notification.</param>
        /// <param name="entityType">Related entity type with this notification (optional).</param>
        /// <param name="displayName">Display name of the notification.</param>
        /// <param name="description">Description for the notification</param>
        /// <param name="permissionDependency">A permission dependency. This notification will be available to a user if this dependency is satisfied.</param>
        /// <param name="featureDependency">A feature dependency. This notification will be available to a tenant if this feature is enabled.</param>
        public NotificationDefinition(string name, Type entityType = null, ILocalizableString displayName = null, ILocalizableString description = null, IPermissionDependency permissionDependency = null, IFeatureDependency featureDependency = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("name", "name can not be null, empty or whitespace!");
            }
            
            Name = name;
            EntityType = entityType;
            DisplayName = displayName;
            Description = description;
            PermissionDependency = permissionDependency;
            FeatureDependency = featureDependency;

            Attributes = new Dictionary<string, object>();
        }
    }
}
