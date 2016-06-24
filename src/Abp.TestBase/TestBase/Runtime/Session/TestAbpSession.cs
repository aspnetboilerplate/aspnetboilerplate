using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Runtime.Session;

namespace Abp.TestBase.Runtime.Session
{
    public class TestAbpSession : IAbpSession, ISingletonDependency
    {
        public long? UserId { get; set; }

        public int? TenantId
        {
            get
            {
                if (!_multiTenancy.IsEnabled)
                {
                    return 1;
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

        public MultiTenancySides MultiTenancySide { get { return GetCurrentMultiTenancySide(); } }
        
        public long? ImpersonatorUserId { get; set; }
        
        public int? ImpersonatorTenantId { get; set; }

        private readonly IMultiTenancyConfig _multiTenancy;
        private int? _tenantId;

        public TestAbpSession(IMultiTenancyConfig multiTenancy)
        {
            _multiTenancy = multiTenancy;
        }

        private MultiTenancySides GetCurrentMultiTenancySide()
        {
            return _multiTenancy.IsEnabled && !TenantId.HasValue
                ? MultiTenancySides.Host
                : MultiTenancySides.Tenant;
        }
    }
}