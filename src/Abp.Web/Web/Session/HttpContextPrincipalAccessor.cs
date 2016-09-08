using System.Security.Claims;
using System.Web;
using Abp.Runtime.Session;

namespace Abp.Web.Session
{
    public class HttpContextPrincipalAccessor : DefaultPrincipalAccessor
    {
        public override ClaimsPrincipal Principal => HttpContext.Current?.User as ClaimsPrincipal ?? base.Principal;
    }
}
