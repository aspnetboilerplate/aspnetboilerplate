using System;
using System.Linq;
using System.Security.Claims;
using Abp.Configuration.Startup;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;

namespace Abp.AspNetCore.Runtime.Session
{
    public class AspNetCoreClaimsAbpSession : IAbpSession
    {
        private const int DefaultTenantId = 1;

        public IHttpContextAccessor HttpContextAccessor { get; set; }

        public AspNetCoreClaimsAbpSession(IMultiTenancyConfig multiTenancy)
        {
            _multiTenancy = multiTenancy;
        }
        private readonly IMultiTenancyConfig _multiTenancy;

        public ClaimsIdentity Identity => Principal?.Identity as ClaimsIdentity;

        public ClaimsPrincipal Principal => HttpContextAccessor?.HttpContext?.User;

        public virtual string UserIdClaimType { get; set; } = ClaimTypes.NameIdentifier;

        public long? UserId
        {
            get
            {

                var userIdClaim = Identity?.Claims.FirstOrDefault(c =>
                    c.Type == UserIdClaimType);
                if (string.IsNullOrEmpty(userIdClaim?.Value))
                {
                    return null;
                }

                long userId;
                if (!long.TryParse(userIdClaim.Value, out userId))
                {
                    return null;
                }

                return userId;
            }
        }

        public virtual int? TenantId
        {
            get
            {
                if (!_multiTenancy.IsEnabled)
                {
                    return DefaultTenantId;
                }


                var tenantIdClaim = Principal.Claims.FirstOrDefault(c =>
                    c.Type == AbpClaimTypes.TenantId);
                if (string.IsNullOrEmpty(tenantIdClaim?.Value))
                {
                    return null;
                }

                return Convert.ToInt32(tenantIdClaim.Value);
            }
        }

        public virtual MultiTenancySides MultiTenancySide =>
            _multiTenancy.IsEnabled && !TenantId.HasValue
            ? MultiTenancySides.Host
            : MultiTenancySides.Tenant;

        public virtual long? ImpersonatorUserId
        {
            get
            {
                var impersonatorUserIdClaim = Principal.Claims.FirstOrDefault(c =>
                    c.Type == AbpClaimTypes.ImpersonatorUserId);
                if (string.IsNullOrEmpty(impersonatorUserIdClaim?.Value))
                {
                    return null;
                }

                return Convert.ToInt64(impersonatorUserIdClaim.Value);
            }
        }

        public virtual int? ImpersonatorTenantId
        {
            get
            {
                if (!_multiTenancy.IsEnabled)
                {
                    return DefaultTenantId;
                }

                var impersonatorTenantIdClaim = Principal.Claims.FirstOrDefault(c =>
                    c.Type == AbpClaimTypes.ImpersonatorTenantId);
                if (string.IsNullOrEmpty(impersonatorTenantIdClaim?.Value))
                {
                    return null;
                }

                return Convert.ToInt32(impersonatorTenantIdClaim.Value);
            }
        }

    }
}
