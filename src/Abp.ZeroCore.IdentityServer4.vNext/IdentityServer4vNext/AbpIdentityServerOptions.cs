using System.IdentityModel.Tokens.Jwt;
using Abp.Runtime.Security;

namespace Abp.IdentityServer4vNext
{
    public class AbpIdentityServerOptions
    {
        /// <summary>
        /// Updates <see cref="JwtSecurityTokenHandler.DefaultInboundClaimTypeMap"/> to be compatible with identity server claims.
        /// Default: true.
        /// </summary>
        public bool UpdateJwtSecurityTokenHandlerDefaultInboundClaimTypeMap { get; set; } = true;

        /// <summary>
        /// Updates <see cref="AbpClaimTypes"/> to be compatible with identity server claims.
        /// Default: true.
        /// </summary>
        public bool UpdateAbpClaimTypes { get; set; } = true;
    }
}
