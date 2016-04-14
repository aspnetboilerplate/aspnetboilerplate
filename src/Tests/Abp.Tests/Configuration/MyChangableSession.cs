using Abp.MultiTenancy;
using Abp.Runtime.Session;
using System;

namespace Abp.Tests.Configuration
{
    public class MyChangableSession : IAbpSession
    {
        public Guid? UserId { get; set; }

        public Guid? TenantId { get; set; }

        public MultiTenancySides MultiTenancySide
        {
            get
            {
                return !TenantId.HasValue ? MultiTenancySides.Host : MultiTenancySides.Tenant;
            }
        }

        public Guid? ImpersonatorUserId { get; set; }

        public Guid? ImpersonatorTenantId { get; set; }
    }
}