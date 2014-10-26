using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Localization;
using Castle.Core.Logging;

namespace Abp.Authorization
{
    /// <summary>
    /// Permission manager.
    /// </summary>
    internal class PermissionManager : IPermissionManager, ISingletonDependency
    {
        public IPermissionGrantStore PermissionGrantStore { get; set; }

        public ILogger Logger { get; set; }

        private readonly IIocManager _iocManager;
        private readonly IAuthorizationConfiguration _authorizationConfiguration;

        private readonly Dictionary<string, PermissionGroup> _rootGroups;
        private readonly PermissionDictionary _permissions;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        public PermissionManager(IIocManager iocManager, IAuthorizationConfiguration authorizationConfiguration)
        {
            PermissionGrantStore = NullPermissionGrantStore.Instance;
            Logger = NullLogger.Instance;

            _iocManager = iocManager;
            _authorizationConfiguration = authorizationConfiguration;

            _rootGroups = new Dictionary<string, PermissionGroup>();
            _permissions = new PermissionDictionary();
        }

        public void Initialize()
        {
            var context = new PermissionDefinitionContext(_rootGroups);

            foreach (var providerType in _authorizationConfiguration.Providers)
            {
                CreatePermissionProvider(providerType).SetPermissions(context);
            }

            foreach (var rootGroup in _rootGroups.Values)
            {
                _permissions.AddGroupPermissionsRecursively(rootGroup);
            }
        }

        public Permission GetPermissionOrNull(string name)
        {
            return _permissions.GetOrDefault(name);
        }

        public IReadOnlyList<Permission> GetAllPermissions()
        {
            return _permissions.Values.ToImmutableList();
        }

        public IReadOnlyList<PermissionGroup> GetAllRootGroups()
        {
            return _rootGroups.Values.ToImmutableList();
        }

        public bool IsGranted(long userId, string permissionName)
        {
            var permission = GetPermissionOrNull(permissionName);
            if (permission == null)
            {
                Logger.Warn("Permission is not defined: " + permissionName);
                return false;
            }

            return PermissionGrantStore.IsGranted(userId, permissionName);
        }

        public IReadOnlyList<Permission> GetGrantedPermissions(long userId)
        {
            return GetAllPermissions().Where(p => IsGranted(userId, p.Name)).ToImmutableList();
        }

        public PermissionGroup GetRootGroupOrNull(string name)
        {
            return _rootGroups.GetOrDefault(name);
        }

        private AuthorizationProvider CreatePermissionProvider(Type providerType)
        {
            if (!_iocManager.IsRegistered(providerType))
            {
                _iocManager.Register(providerType);
            }

            return (AuthorizationProvider) _iocManager.Resolve(providerType);
        }

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

                return _rootGroups[name] = new PermissionGroup(name, displayName);
            }

            public PermissionGroup GetRootGroupOrNull(string name)
            {
                return _rootGroups.GetOrDefault(name);
            }
        }
    }
}