using System.Security.Claims;

namespace Abp.Runtime.Security
{
    /// <summary>
    /// Used to get ABP-specific claim type names.
    /// </summary>
    public static class AbpClaimTypes
    {
        /// <summary>
        /// UserId.
        /// Default: <see cref="ClaimTypes.Name"/>
        /// </summary>
        public static string UserName { get; set; } = ClaimTypes.Name;

        /// <summary>
        /// UserId.
        /// Default: <see cref="ClaimTypes.NameIdentifier"/>
        /// </summary>
        public static string UserId { get; set; } = ClaimTypes.NameIdentifier;

        /// <summary>
        /// UserId.
        /// Default: <see cref="ClaimTypes.Role"/>
        /// </summary>
        public static string Role { get; set; } = ClaimTypes.Role;

        /// <summary>
        /// TenantId.
        /// Default: http://www.aspnetboilerplate.com/identity/claims/tenantId
        /// </summary>
        public static string TenantId { get; set; } = "http://www.aspnetboilerplate.com/identity/claims/tenantId";

        /// <summary>
        /// ImpersonatorUserId.
        /// Default: http://www.aspnetboilerplate.com/identity/claims/impersonatorUserId
        /// </summary>
        public static string ImpersonatorUserId { get; set; } = "http://www.aspnetboilerplate.com/identity/claims/impersonatorUserId";

        /// <summary>
        /// ImpersonatorTenantId
        /// Default: http://www.aspnetboilerplate.com/identity/claims/impersonatorTenantId
        /// </summary>
        public static string ImpersonatorTenantId { get; set; } = "http://www.aspnetboilerplate.com/identity/claims/impersonatorTenantId";
    }
}
