using Adorable.MultiTenancy;
using Adorable.Runtime.Session;

namespace Adorable.Tests.Configuration
{
    public class MyChangableSession : IAbpSession
    {
        public long? UserId { get; set; }

        public int? TenantId { get; set; }

        public MultiTenancySides MultiTenancySide
        {
            get
            {
                return !TenantId.HasValue ? MultiTenancySides.Host : MultiTenancySides.Tenant;
            }
        }

        public long? ImpersonatorUserId { get; set; }

        public int? ImpersonatorTenantId { get; set; }
    }
}