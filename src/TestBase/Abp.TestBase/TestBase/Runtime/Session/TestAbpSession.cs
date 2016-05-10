using System;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Runtime.Session;

namespace Abp.TestBase.Runtime.Session
{
    public class TestAbpSession : IAbpSession, ISingletonDependency
    {
        private readonly IMultiTenancyConfig _multiTenancy;
        private Guid? _tenantId;

        public TestAbpSession(IMultiTenancyConfig multiTenancy)
        {
            _multiTenancy = multiTenancy;
        }

        public Guid? UserId { get; set; }

        public Guid? TenantId
        {
            get
            {
                if (!_multiTenancy.IsEnabled)
                {
                    return Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001");
                }

                return _tenantId;
            }
            set
            {
                if (!_multiTenancy.IsEnabled && value != Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-000000000001"))
                {
                    throw new AbpException(
                        "Can not set TenantId since multi-tenancy is not enabled. Use IMultiTenancyConfig.IsEnabled to enable it.");
                }

                _tenantId = value;
            }
        }

        public MultiTenancySides MultiTenancySide
        {
            get { return GetCurrentMultiTenancySide(); }
        }

        public Guid? ImpersonatorUserId { get; set; }

        public Guid? ImpersonatorTenantId { get; set; }

        private MultiTenancySides GetCurrentMultiTenancySide()
        {
            return _multiTenancy.IsEnabled && !TenantId.HasValue
                ? MultiTenancySides.Host
                : MultiTenancySides.Tenant;
        }
    }
}