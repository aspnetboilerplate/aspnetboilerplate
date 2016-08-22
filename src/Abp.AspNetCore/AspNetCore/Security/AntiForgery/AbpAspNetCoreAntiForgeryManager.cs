using Abp.Threading;
using Abp.Web.Security.AntiForgery;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http;

namespace Abp.AspNetCore.Security.AntiForgery
{
    public class AbpAspNetCoreAntiForgeryManager : AbpAntiForgeryManager
    {
        private readonly IAntiforgery _antiforgery;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AbpAspNetCoreAntiForgeryManager(
            IAbpAntiForgeryConfiguration configuration,
            IAntiforgery antiforgery,
            IHttpContextAccessor httpContextAccessor
        )
            : base(configuration)
        {
            _antiforgery = antiforgery;
            _httpContextAccessor = httpContextAccessor;
        }

        public override string GenerateToken()
        {
            return _antiforgery.GetAndStoreTokens(_httpContextAccessor.HttpContext).RequestToken;
        }

        public override bool IsValid(string cookieValue, string tokenValue)
        {
            return AsyncHelper.RunSync(() => _antiforgery.IsRequestValidAsync(_httpContextAccessor.HttpContext));
        }
    }
}