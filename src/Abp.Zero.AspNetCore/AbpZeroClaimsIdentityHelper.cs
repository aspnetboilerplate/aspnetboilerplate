using System;
using System.Security.Claims;
using System.Security.Principal;
using Abp.Runtime.Security;
using Microsoft.AspNet.Identity;

namespace Abp.Zero.AspNetCore
{
    internal static class AbpZeroClaimsIdentityHelper
    {
        public static int? GetTenantId(IIdentity identity)
        {
            if (identity == null)
            {
                return null;
            }

            var claimsIdentity = identity as ClaimsIdentity;

            var tenantIdOrNull = claimsIdentity?.FindFirstValue(AbpClaimTypes.TenantId);
            if (tenantIdOrNull == null)
            {
                return null;
            }

            return Convert.ToInt32(tenantIdOrNull);
        }
    }
}