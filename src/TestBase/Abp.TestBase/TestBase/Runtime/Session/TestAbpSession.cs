using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.MultiTenancy;
using Abp.Runtime.Session;

namespace Abp.TestBase.Runtime.Session
{
    public class TestAbpSession : IAbpSession, ISingletonDependency
    {
        public long? UserId { get; set; }

        public int? TenantId { get; set; }

        public MultiTenancySide MultiTenancySide { get { return GetCurrentMultiTenancySide(); } }

        private readonly IMultiTenancyConfig _multiTenancy;

        public TestAbpSession(IMultiTenancyConfig multiTenancy)
        {
            _multiTenancy = multiTenancy;

        }

        private MultiTenancySide GetCurrentMultiTenancySide()
        {
            return _multiTenancy.IsEnabled && !TenantId.HasValue
                ? MultiTenancySide.Host
                : MultiTenancySide.Tenant;
        }
    }
}