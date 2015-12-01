using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using Abp.Configuration.Startup;
using Abp.MultiTenancy;
using Abp.Runtime.Security;

namespace Abp.Runtime.Session
{
    /// <summary>
    /// Implements <see cref="IAbpSession"/> to get session properties from claims of <see cref="Thread.CurrentPrincipal"/>.
    /// </summary>
    public class ClaimsAbpSession : IAbpSession
    {
        private const int DefaultTenantId = 1;

        public virtual long? UserId
        {
            get
            {
                var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
                if (claimsPrincipal == null)
                {
                    return null;
                }

                var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
                if (claimsIdentity == null)
                {
                    return null;
                }

                var userIdClaim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
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

                var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
                if (claimsPrincipal == null)
                {
                    return null;
                }

                var tenantIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == AbpClaimTypes.TenantId);
                if (tenantIdClaim == null || string.IsNullOrEmpty(tenantIdClaim.Value))
                {
                    return null;
                }

                return Convert.ToInt32(tenantIdClaim.Value);
            }
        }

        public virtual MultiTenancySides MultiTenancySide
        {
            get
            {
                return _multiTenancy.IsEnabled && !TenantId.HasValue
                    ? MultiTenancySides.Host
                    : MultiTenancySides.Tenant;
            }
        }

        public virtual long? ImpersonatorUserId
        {
            get
            {
                var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
                if (claimsPrincipal == null)
                {
                    return null;
                }

                var impersonatorUserIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == AbpClaimTypes.ImpersonatorUserId);
                if (impersonatorUserIdClaim == null || string.IsNullOrEmpty(impersonatorUserIdClaim.Value))
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

                var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
                if (claimsPrincipal == null)
                {
                    return null;
                }

                var impersonatorTenantIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == AbpClaimTypes.ImpersonatorTenantId);
                if (impersonatorTenantIdClaim == null || string.IsNullOrEmpty(impersonatorTenantIdClaim.Value))
                {
                    return null;
                }

                return Convert.ToInt32(impersonatorTenantIdClaim.Value);
            }
        }

        private readonly IMultiTenancyConfig _multiTenancy;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ClaimsAbpSession(IMultiTenancyConfig multiTenancy)
        {
            _multiTenancy = multiTenancy;
        }
    }
}