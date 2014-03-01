using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Exceptions;
using Abp.Localization;

namespace Abp.Configuration
{
    /// <summary>
    /// A setting group is used to group some settings togehter.
    /// A group can be child of another group and can has child groups.
    /// </summary>
    public class SettingGroup
    {
        /// <summary>
        /// Unique name of the setting group.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Display name of the setting.
        /// This can be used to show setting to the user.
        /// </summary>
        public LocalizableString DisplayName { get; private set; }

        /// <summary>
        /// Gets parent of this group.
        /// </summary>
        public SettingGroup Parent { get; private set; }

        /// <summary>
        /// Gets a list of all children of this group.
        /// </summary>
        public IReadOnlyList<SettingGroup> Children
        {
            get { return _children.ToImmutableList(); }
        }

        private readonly List<SettingGroup> _children;

        /// <summary>
        /// Creates a new <see cref="SettingGroup"/> object.
        /// </summary>
        /// <param name="name">Unique name of the setting group</param>
        /// <param name="displayName">Display name of the setting</param>
        public SettingGroup(string name, LocalizableString displayName)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("name parameter is invalid! It can not be null or empty or whitespace", "name"); //TODO: Simpify throwing such exceptions
            }

            Name = name;
            DisplayName = displayName;
            _children = new List<SettingGroup>();
        }

        /// <summary>
        /// Adds a <see cref="SettingGroup"/> as child of this group.
        /// </summary>
        /// <param name="child">Child to be added</param>
        /// <returns>This child group to be able to add more child</returns>
        public SettingGroup AddChild(SettingGroup child)
        {
            if (child.Parent != null)
            {
                throw new AbpException("Setting group " + child.Name + " has already a Parent (" + child.Parent.Name + ").");
            }

            _children.Add(child);
            child.Parent = this;
            return this;
        }
    }
}