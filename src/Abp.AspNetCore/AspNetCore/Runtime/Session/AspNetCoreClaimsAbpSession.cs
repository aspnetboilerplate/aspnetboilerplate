using System.Security.Claims;
using Abp.Configuration.Startup;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;

namespace Abp.AspNetCore.Runtime.Session
{
    public class AspNetCoreClaimsAbpSession : ClaimsAbpSession
    {
        public IHttpContextAccessor HttpContextAccessor { get; set; }

        protected override ClaimsPrincipal Principal => HttpContextAccessor?.HttpContext?.User;

        public AspNetCoreClaimsAbpSession(IMultiTenancyConfig multiTenancy)
            : base(multiTenancy)
        {
        }
    }
}
