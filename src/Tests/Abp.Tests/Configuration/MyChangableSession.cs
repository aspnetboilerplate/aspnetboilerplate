using Abp.MultiTenancy;
using Abp.Runtime.Session;

namespace Abp.Tests.Configuration
{
    public class MyChangableSession : IAbpSession
    {
        public long? UserId { get; set; }

        public int? TenantId { get; set; }

        public MultiTenancySides MultiTenancySides
        {
            get
            {
                return !TenantId.HasValue ? MultiTenancySides.Host : MultiTenancySides.Tenant;
            }
        }
    }
}