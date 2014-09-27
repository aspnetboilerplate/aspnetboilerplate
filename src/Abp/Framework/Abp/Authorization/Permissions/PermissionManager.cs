using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Dependency;
using Abp.Localization;
using Abp.Startup;
using Abp.Utils.Extensions.Collections;

namespace Abp.Authorization.Permissions
{
    /// <summary>
    /// Permission manager.
    /// </summary>
    public class PermissionManager : IPermissionManager, ISingletonDependency
    {
        private readonly IIocManager _iocManager;
        private readonly IPermissionProviderFinder _providerFinder;

        private readonly Dictionary<string, Permission> _permissions;
        private readonly Dictionary<string, PermissionGroup> _rootGroups;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PermissionManager(IIocManager iocManager, IPermissionProviderFinder providerFinder)
        {
            _iocManager = iocManager;
            _providerFinder = providerFinder;
            _permissions = new Dictionary<string, Permission>();
            _rootGroups = new Dictionary<string, PermissionGroup>();
            Initialize();
        }

        public Permission GetPermissionOrNull(string permissionName)
        {
            return _permissions.GetOrDefault(permissionName);
        }

        public IReadOnlyList<Permission> GetAllPermissions()
        {
            return _permissions.Values.ToImmutableList();
        }

        public IReadOnlyList<string> GetAllPermissionNames()
        {
            return _permissions.Values.Select(p => p.Name).ToImmutableList();
        }

        public IReadOnlyList<PermissionGroup> GetPermissionGroups()
        {
            return _rootGroups.Values.ToImmutableList();
        }

        private void Initialize()
        {
            var context = new PermissionDefinitionContext(_rootGroups);

            var providerTypes = _providerFinder.FindAll();

            foreach (var providerType in providerTypes)
            {
                if (!_iocManager.IsRegistered(providerType))
                {
                    _iocManager.Register(providerType);
                }

                var providerObj = (IPermissionProvider)_iocManager.Resolve(providerType);
                providerObj.DefinePermissions(context);
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

        private void AddPermissionRecursively(Permission permission)
        {
            //Did defined before?
            Permission existingPermission;
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

            public PermissionGroup CreateRootGroup(string name, ILocalizableString displayName)
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