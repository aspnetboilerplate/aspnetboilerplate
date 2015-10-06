using Abp.MultiTenancy;
using Abp.Runtime.Session;

namespace Abp.Tests.Configuration
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