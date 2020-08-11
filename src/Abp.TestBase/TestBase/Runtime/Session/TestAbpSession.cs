using System;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Runtime;
using Abp.Runtime.Session;

namespace Abp.TestBase.Runtime.Session
{
    public class TestAbpSession : IAbpSession, ISingletonDependency
    {
        public virtual long? UserId
        {
            get
            {
                if (_sessionOverrideScopeProvider.GetValue(AbpSessionBase.SessionOverrideContextKey) != null)
                {
                    return _sessionOverrideScopeProvider.GetValue(AbpSessionBase.SessionOverrideContextKey).UserId;
                }

                return _userId;
            }
            set { _userId = value; }
        }

        public virtual int? TenantId
        {
            get
            {
                if (!_multiTenancy.IsEnabled)
                {
                    return 1;
                }

                if (_sessionOverrideScopeProvider.GetValue(AbpSessionBase.SessionOverrideContextKey) != null)
                {
                    return _sessionOverrideScopeProvider.GetValue(AbpSessionBase.SessionOverrideContextKey).TenantId;
                }

                var resolvedValue = _tenantResolver.ResolveTenantId();
                if (resolvedValue != null)
                {
                    return resolvedValue;
                }

                return _tenantId;
            }
            set
            {
                if (!_multiTenancy.IsEnabled && value != 1 && value != null)
                {
                    throw new AbpException("Can not set TenantId since multi-tenancy is not enabled. Use IMultiTenancyConfig.IsEnabled to enable it.");
                }

                _tenantId = value;
            }
        }

        public virtual long? BranchId
        {
            get
            {
                if (!_multiTenancy.IsEnabled)
                {
                    return 1;
                }

                if (_sessionOverrideScopeProvider.GetValue(AbpSessionBase.SessionOverrideContextKey) != null)
                {
                    return _sessionOverrideScopeProvider.GetValue(AbpSessionBase.SessionOverrideContextKey).BranchId;
                }

                var resolvedValue = _branchResolver.ResolveBranchId();
                if (resolvedValue != null)
                {
                    return resolvedValue;
                }

                return _branchId;
            }
            set
            {
                if (!_multiTenancy.IsEnabled && value != 1 && value != null)
                {
                    throw new AbpException("Can not set BranchId since multi-tenancy is not enabled. Use IMultiTenancyConfig.IsEnabled to enable it.");
                }

                _branchId = value;
            }
        }

        public virtual MultiTenancySides MultiTenancySide { get { return GetCurrentMultiTenancySide(); } }
        
        public virtual long? ImpersonatorUserId { get; set; }
        
        public virtual int? ImpersonatorTenantId { get; set; }

        private readonly IMultiTenancyConfig _multiTenancy;
        private readonly IAmbientScopeProvider<SessionOverride> _sessionOverrideScopeProvider;
        private readonly ITenantResolver _tenantResolver;
        private readonly IBranchResolver _branchResolver;
        private int? _tenantId;
        private long? _userId;
        private long? _branchId;

        public TestAbpSession(
            IMultiTenancyConfig multiTenancy, 
            IAmbientScopeProvider<SessionOverride> sessionOverrideScopeProvider,
            ITenantResolver tenantResolver,
            IBranchResolver branchResolver)
        {
            _multiTenancy = multiTenancy;
            _sessionOverrideScopeProvider = sessionOverrideScopeProvider;
            _tenantResolver = tenantResolver;
            _branchResolver = branchResolver;
        }

        protected virtual MultiTenancySides GetCurrentMultiTenancySide()
        {
            return _multiTenancy.IsEnabled && !TenantId.HasValue
                ? MultiTenancySides.Host
                : MultiTenancySides.Tenant;
        }

        public virtual IDisposable Use(int? tenantId, long? userId, long? branchId)
        {
            return _sessionOverrideScopeProvider.BeginScope(AbpSessionBase.SessionOverrideContextKey, new SessionOverride(tenantId, userId, branchId));
        }
    }
}