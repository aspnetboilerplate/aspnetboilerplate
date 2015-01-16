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
    internal class PermissionManager : IPermissionManager, ISingletonDependency, IPermissionDefinitionContext
    {
        public IPermissionGrantStore PermissionGrantStore { get; set; }
        public ILogger Logger { get; set; }

        private readonly IIocManager _iocManager;
        private readonly IAuthorizationConfiguration _authorizationConfiguration;

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

            _permissions = new PermissionDictionary();
        }

        public void Initialize()
        {
            foreach (var providerType in _authorizationConfiguration.Providers)
            {
                CreatePermissionProvider(providerType).SetPermissions(this);
            }

            _permissions.AddAllPermissions();
        }

        public Permission CreatePermission(string name, ILocalizableString displayName, bool isGrantedByDefault = false, ILocalizableString description = null)
        {
            if (_permissions.ContainsKey(name))
            {
                throw new AbpException("There is already a permission with name: " + name);
            }

            var permission = new Permission(name, displayName, isGrantedByDefault, description);
            _permissions[permission.Name] = permission;
            return permission;
        }

        public Permission GetPermissionOrNull(string name)
        {
            return _permissions.GetOrDefault(name);
        }

        public IReadOnlyList<Permission> GetAllPermissions()
        {
            return _permissions.Values.ToImmutableList();
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

        private AuthorizationProvider CreatePermissionProvider(Type providerType)
        {
            if (!_iocManager.IsRegistered(providerType))
            {
                _iocManager.Register(providerType);
            }

            return (AuthorizationProvider) _iocManager.Resolve(providerType);
        }
    }
}