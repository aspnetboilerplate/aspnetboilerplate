using System.Collections.Generic;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Collections.Extensions;
using Abp.Localization;
using System;

namespace Abp.Application.Navigation
{
    /// <summary>
    /// Represents an item in a <see cref="MenuDefinition"/>.
    /// </summary>
    public class MenuItemDefinition : IHasMenuItemDefinitions
    {
        /// <summary>
        /// Unique name of the menu item in the application. 
        /// Can be used to find this menu item later.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Display name of the menu item. Required.
        /// </summary>
        public ILocalizableString DisplayName { get; set; }

        /// <summary>
        /// The Display order of the menu. Optional.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Icon of the menu item if exists. Optional.
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// The URL to navigate when this menu item is selected. Optional.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// A permission name. Only users that has this permission can see this menu item.
        /// Optional.
        /// </summary>
        [Obsolete("Use PermissionDependency instead.")]
        public string RequiredPermissionName { get; set; }

        /// <summary>
        /// A permission dependency. Only users that can satisfy this permission dependency can see this menu item.
        /// Optional.
        /// </summary>
        public IPermissionDependency PermissionDependency { get; set; }

        /// <summary>
        /// A feature dependency.
        /// Optional.
        /// </summary>
        public IFeatureDependency FeatureDependency { get; set; }

        /// <summary>
        /// This can be set to true if only authenticated users should see this menu item.
        /// </summary>
        public bool RequiresAuthentication { get; set; }

        /// <summary>
        /// Returns true if this menu item has no child <see cref="Items"/>.
        /// </summary>
        public bool IsLeaf => Items.IsNullOrEmpty();

        /// <summary>
        /// Target of the menu item. Can be "_blank", "_self", "_parent", "_top" or a frame name.
        /// </summary>
        public string Target { get; set; }

        /// <summary>
        /// Can be used to store a custom object related to this menu item. Optional.
        /// </summary>
        public object CustomData { get; set; }

        /// <summary>
        /// Can be used to enable/disable a menu item.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Can be used to show/hide a menu item.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Sub items of this menu item. Optional.
        /// </summary>
        public virtual List<MenuItemDefinition> Items { get; }

        /// <param name="name"></param>
        /// <param name="displayName"></param>
        /// <param name="icon"></param>
        /// <param name="url"></param>
        /// <param name="requiresAuthentication"></param>
        /// <param name="requiredPermissionName">This parameter is obsolete. Use <paramref name="permissionDependency"/> instead!</param>
        /// <param name="order"></param>
        /// <param name="customData"></param>
        /// <param name="featureDependency"></param>
        /// <param name="target"></param>
        /// <param name="isEnabled"></param>
        /// <param name="isVisible"></param>
        /// <param name="permissionDependency"></param>
        public MenuItemDefinition(
            string name,
            ILocalizableString displayName,
            string icon = null,
            string url = null,
            bool requiresAuthentication = false,
            string requiredPermissionName = null,
            int order = 0,
            object customData = null,
            IFeatureDependency featureDependency = null,
            string target = null,
            bool isEnabled = true,
            bool isVisible = true,
            IPermissionDependency permissionDependency = null)
        {
            Check.NotNull(name, nameof(name));
            Check.NotNull(displayName, nameof(displayName));

            Name = name;
            DisplayName = displayName;
            Icon = icon;
            Url = url;
            RequiresAuthentication = requiresAuthentication;
            RequiredPermissionName = requiredPermissionName;
            Order = order;
            CustomData = customData;
            FeatureDependency = featureDependency;
            Target = target;
            IsEnabled = isEnabled;
            IsVisible = isVisible;
            PermissionDependency = permissionDependency;

            Items = new List<MenuItemDefinition>();
        }

        /// <summary>
        /// Adds a <see cref="MenuItemDefinition"/> to <see cref="Items"/>.
        /// </summary>
        /// <param name="menuItem"><see cref="MenuItemDefinition"/> to be added</param>
        /// <returns>This <see cref="MenuItemDefinition"/> object</returns>
        public MenuItemDefinition AddItem(MenuItemDefinition menuItem)
        {
            Items.Add(menuItem);
            return this;
        }

        /// <summary>
        /// Remove notification with given name
        /// </summary>
        /// <param name="name"></param>
        public void RemoveItem(string name)
        {
            Items.RemoveAll(m => m.Name == name);
        }
    }
}
