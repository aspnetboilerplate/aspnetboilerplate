using Abp.Configuration.Startup;
using Abp.MultiTenancy;
using Abp.Runtime.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;

namespace Abp.Runtime.Session
{
    /// <summary>
    /// Implements <see cref="IAbpSession"/> to get session properties from claims of <see cref="Thread.CurrentPrincipal"/>.
    /// </summary>
    public class ClaimsAbpSession : IAbpSession
    {
        private const string DefaultTenantId = "FFFFFFFF-FFFF-FFFF-FFFF-000000000001";

        public virtual Guid? UserId
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

                Guid userId;
                if (!Guid.TryParse(userIdClaim.Value, out userId))
                {
                    return null;
                }

                return userId;
            }
        }

        public virtual Guid? TenantId
        {
            get
            {
                if (!_multiTenancy.IsEnabled)
                {
                    return Guid.Parse(DefaultTenantId);
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

                Guid tenantId;
                if (!Guid.TryParse(tenantIdClaim.Value, out tenantId))
                {
                    return null;
                }

                return tenantId;
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

        public virtual Guid? ImpersonatorUserId
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

                Guid impersonatorUserId;

                if (!Guid.TryParse(impersonatorUserIdClaim.Value, out impersonatorUserId))
                {
                    return null;
                }

                return impersonatorUserId;
            }
        }

        public virtual Guid? ImpersonatorTenantId
        {
            get
            {
                if (!_multiTenancy.IsEnabled)
                {
                    return Guid.Parse(DefaultTenantId);
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

                Guid impersonatorTenantId;
                if (!Guid.TryParse(impersonatorTenantIdClaim.Value, out impersonatorTenantId))
                {
                    return null;
                }

                return impersonatorTenantId;
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