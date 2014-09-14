using System.Collections.Generic;
using System.Collections.Immutable;
using Abp.Dependency;
using Abp.Localization;
using Abp.Startup;
using Abp.Utils.Extensions.Collections;

namespace Abp.Application.Authorization.Permissions
{
    /// <summary>
    /// Permission manager.
    /// </summary>
    public class PermissionManager : IPermissionManager, ISingletonDependency
    {
        private readonly IPermissionDefinitionProviderFinder _definitionProviderFinder;

        private readonly Dictionary<string, PermissionDefinition> _permissions;
        private readonly Dictionary<string, PermissionGroup> _rootGroups;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PermissionManager(IPermissionDefinitionProviderFinder definitionProviderFinder)
        {
            _definitionProviderFinder = definitionProviderFinder;
            _permissions = new Dictionary<string, PermissionDefinition>();
            _rootGroups = new Dictionary<string, PermissionGroup>();
            Initialize();
        }

        public PermissionDefinition GetPermissionOrNull(string permissionName)
        {
            return _permissions.GetOrDefault(permissionName);
        }

        public IReadOnlyList<PermissionDefinition> GetAllPermissions()
        {
            return _permissions.Values.ToImmutableList();
        }

        public IReadOnlyList<PermissionGroup> GetRootPermissionGroups()
        {
            return _rootGroups.Values.ToImmutableList();
        }

        private void Initialize()
        {
            var context = new PermissionDefinitionContext(_rootGroups);

            foreach (var provider in _definitionProviderFinder.GetPermissionProviders())
            {
                provider.DefinePermissions(context);
            }

            foreach (var rootGroup in _rootGroups.Values)
            {
                AddGroupRecursively(rootGroup);
            }
        }

        private void AddGroupRecursively(PermissionGroup permissionGroup)
        {
            //Add permissions of the group
            foreach (var permission in permissionGroup.Permissions)
            {
                AddPermissionRecursively(permission);
            }

            //Add child groups
            foreach (var childPermissionGroup in permissionGroup.Children)
            {
                AddGroupRecursively(childPermissionGroup);
            }
        }

        private void AddPermissionRecursively(PermissionDefinition permission)
        {
            //Did defined before?
            PermissionDefinition existingPermission;
            if (_permissions.TryGetValue(permission.Name, out existingPermission))
            {
                throw new AbpInitializationException("Duplicate permission name detected for " + permission.Name);
            }

            //Add permission
            _permissions[permission.Name] = permission;

            //Add child permissions
            foreach (var childPermission in permission.Children)
            {
                AddPermissionRecursively(childPermission);
            }
        }

        #region PermissionDefinitionContext

        private class PermissionDefinitionContext : IPermissionDefinitionContext
        {
            private readonly Dictionary<string, PermissionGroup> _rootGroups;

            public PermissionDefinitionContext(Dictionary<string, PermissionGroup> rootGroups)
            {
                _rootGroups = rootGroups;
            }

            public PermissionGroup CreateRootGroup(string name, LocalizableString displayName)
            {
                if (_rootGroups.ContainsKey(name))
                {
                    throw new AbpInitializationException(
                        string.Format(
                            "There is already a root permission group named '{0}'. Try to get it using GetRootGroupOrNull method",
                            name));
                }

                var permissionGroup = new PermissionGroup(name, displayName);
                _rootGroups[name] = permissionGroup;
                return permissionGroup;
            }

            public PermissionGroup GetRootGroupOrNull(string name)
            {
                return _rootGroups.GetOrDefault(name);
            }
        }

        #endregion
    }
}