using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Runtime.Session;

namespace Abp.Authorization
{
    /// <summary>
    /// Permission manager.
    /// </summary>
    internal class PermissionManager : PermissionDefinitionContextBase, IPermissionManager, ISingletonDependency
    {
        public IAbpSession AbpSession { get; set; }

        private readonly IIocManager _iocManager;
        private readonly IAuthorizationConfiguration _authorizationConfiguration;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PermissionManager(IIocManager iocManager, IAuthorizationConfiguration authorizationConfiguration)
        {
            _iocManager = iocManager;
            _authorizationConfiguration = authorizationConfiguration;
            AbpSession = NullAbpSession.Instance;
        }

        public void Initialize()
        {
            foreach (var providerType in _authorizationConfiguration.Providers)
            {
                CreateAuthorizationProvider(providerType).SetPermissions(this);
            }

            Permissions.AddAllPermissions();
        }

        public Permission GetPermission(string name)
        {
            var permission = Permissions.GetOrDefault(name);
            if (permission == null)
            {
                throw new AbpException("There is no permission with name: " + name);
            }

            if (!permission.MultiTenancySide.HasFlag(AbpSession.MultiTenancySide))
            {
                throw new AbpException(string.Format("Permission {0} is not marked as {1}", name, AbpSession.MultiTenancySide));
            }

            return permission;
        }

        public IReadOnlyList<Permission> GetAllPermissions()
        {
            var tenancySide = AbpSession.MultiTenancySide;
            return Permissions.Values
                  .Where(p => p.MultiTenancySide.HasFlag(tenancySide))
                  .ToImmutableList();
        }

        private AuthorizationProvider CreateAuthorizationProvider(Type providerType)
        {
            if (!_iocManager.IsRegistered(providerType))
            {
                _iocManager.Register(providerType);
            }

            return (AuthorizationProvider)_iocManager.Resolve(providerType);
        }
    }
}