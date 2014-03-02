using System.Security.Claims;
using System.Threading;

namespace Abp.Modules.Core.Tests.Settings
{
    public static class TestHelper
    {
        public static void SetUserPrincipal(int userId)
        {
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", userId.ToString(), "http://www.w3.org/2001/XMLSchema#string", "LOCAL_AUTHORITY", "LOCAL_AUTHORITY"));
            Thread.CurrentPrincipal = new ClaimsPrincipal(identity);
        }
    }
}