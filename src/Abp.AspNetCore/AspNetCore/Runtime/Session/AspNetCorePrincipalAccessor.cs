using System.Security.Claims;
using Abp.Runtime.Session;
using Microsoft.AspNetCore.Http;

namespace Abp.AspNetCore.Runtime.Session
{
    public class AspNetCorePrincipalAccessor : DefaultPrincipalAccessor
    {
        public override ClaimsPrincipal Principal => _httpContextAccessor.HttpContext?.User ?? base.Principal;

        private readonly HttpContextAccessor _httpContextAccessor;

        public AspNetCorePrincipalAccessor(HttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
    }
}
