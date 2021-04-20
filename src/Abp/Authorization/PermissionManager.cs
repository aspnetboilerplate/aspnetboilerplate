using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Abp.Application.Features;
using Abp.Collections.Extensions;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Runtime.Session;

namespace Abp.Authorization
{
    /// <summary>
    /// Permission manager.
    /// </summary>
    public class PermissionManager : PermissionDefinitionContextBase, IPermissionManager, ISingletonDependency
    {
        public IAbpSession AbpSession { get; set; }

        private readonly IIocManager _iocManager;
        private readonly IAuthorizationConfiguration _authorizationConfiguration;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IMultiTenancyConfig _multiTenancy;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PermissionManager(
            IIocManager iocManager,
            IAuthorizationConfiguration authorizationConfiguration,
            IUnitOfWorkManager unitOfWorkManager, 
            IMultiTenancyConfig multiTenancy)
        {
            _iocManager = iocManager;
            _authorizationConfiguration = authorizationConfiguration;
            _unitOfWorkManager = unitOfWorkManager;
            _multiTenancy = multiTenancy;            

            AbpSession = NullAbpSession.Instance;
        }

        public virtual void Initialize()
        {
            foreach (var providerType in _authorizationConfiguration.Providers)
            {
                using (var provider = _iocManager.ResolveAsDisposable<AuthorizationProvider>(providerType))
                {
                    provider.Object.SetPermissions(this);
                }
            }

            Permissions.AddAllPermissions();
        }

        public virtual Permission GetPermission(string name)
        {
            var permission = Permissions.GetOrDefault(name);
            if (permission == null)
            {
                throw new AbpException("There is no permission with name: " + name);
            }

            return permission;
        }

        public virtual IReadOnlyList<Permission> GetAllPermissions(bool tenancyFilter = true)
        {
            using (var featureDependencyContext = _iocManager.ResolveAsDisposable<FeatureDependencyContext>())
            {
                var featureDependencyContextObject = featureDependencyContext.Object;
                featureDependencyContextObject.TenantId = GetCurrentTenantId();

                return Permissions.Values
                    .WhereIf(tenancyFilter, p => p.MultiTenancySides.HasFlag(GetCurrentMultiTenancySide()))
                    .Where(p =>
                        p.FeatureDependency == null ||
                        GetCurrentMultiTenancySide() == MultiTenancySides.Host ||
                        p.FeatureDependency.IsSatisfied(featureDependencyContextObject)
                    ).ToImmutableList();
            }
        }

        public virtual IReadOnlyList<Permission> GetAllPermissions(MultiTenancySides multiTenancySides)
        {
            using (var featureDependencyContext = _iocManager.ResolveAsDisposable<FeatureDependencyContext>())
            {
                var featureDependencyContextObject = featureDependencyContext.Object;
                featureDependencyContextObject.TenantId = GetCurrentTenantId();

                return Permissions.Values
                    .Where(p => p.MultiTenancySides.HasFlag(multiTenancySides))
                    .Where(p =>
                        p.FeatureDependency == null ||
                        GetCurrentMultiTenancySide() == MultiTenancySides.Host ||
                        (p.MultiTenancySides.HasFlag(MultiTenancySides.Host) &&
                         multiTenancySides.HasFlag(MultiTenancySides.Host)) ||
                        p.FeatureDependency.IsSatisfied(featureDependencyContextObject)
                    ).ToImmutableList();
            }
        }
        
        private MultiTenancySides GetCurrentMultiTenancySide()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _multiTenancy.IsEnabled && !_unitOfWorkManager.Current.GetTenantId().HasValue
                    ? MultiTenancySides.Host
                    : MultiTenancySides.Tenant;
            }

            return AbpSession.MultiTenancySide;
        }

        private int? GetCurrentTenantId()
        {
            if (_unitOfWorkManager.Current != null)
            {
                return _unitOfWorkManager.Current.GetTenantId();
            }

            return AbpSession.TenantId;
        }
    }
}
