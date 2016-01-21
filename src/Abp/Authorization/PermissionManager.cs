using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Application.Features;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.MultiTenancy;
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
        private readonly FeatureDependencyContext _featureDependencyContext;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PermissionManager(
            IIocManager iocManager, 
            IAuthorizationConfiguration authorizationConfiguration,
            FeatureDependencyContext featureDependencyContext
            )
        {
            _iocManager = iocManager;
            _authorizationConfiguration = authorizationConfiguration;
            _featureDependencyContext = featureDependencyContext;

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

            return permission;
        }

        public IReadOnlyList<Permission> GetAllPermissions(bool tenancyFilter = true)
        {
            return Permissions.Values
                .WhereIf(tenancyFilter, p => p.MultiTenancySides.HasFlag(AbpSession.MultiTenancySide))
                .Where(p =>
                    p.FeatureDependency == null ||
                    AbpSession.MultiTenancySide == MultiTenancySides.Host ||
                    p.FeatureDependency.IsSatisfied(_featureDependencyContext)
                ).ToImmutableList();
        }

        public IReadOnlyList<Permission> GetAllPermissions(MultiTenancySides multiTenancySides)
        {
            return Permissions.Values
                .Where(p => p.MultiTenancySides.HasFlag(multiTenancySides))
                .Where(p =>
                    p.FeatureDependency == null ||
                    AbpSession.MultiTenancySide == MultiTenancySides.Host ||
                    (p.MultiTenancySides.HasFlag(MultiTenancySides.Host) && multiTenancySides.HasFlag(MultiTenancySides.Host)) ||
                    p.FeatureDependency.IsSatisfied(_featureDependencyContext)
                ).ToImmutableList();
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