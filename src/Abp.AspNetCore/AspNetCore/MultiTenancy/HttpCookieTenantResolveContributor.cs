using Abp.Dependency;
using Abp.Extensions;
using Abp.MultiTenancy;
using Microsoft.AspNetCore.Http;

namespace Abp.AspNetCore.MultiTenancy
{
    public class HttpCookieTenantResolveContributor : ITenantResolveContributor, ITransientDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HttpCookieTenantResolveContributor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int? ResolveTenantId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            var tenantIdValue = httpContext.Request.Cookies[MultiTenancyConsts.TenantIdResolveKey];
            if (tenantIdValue.IsNullOrEmpty())
            {
                return null;
            }

            int tenantId;
            return !int.TryParse(tenantIdValue, out tenantId) ? (int?) null : tenantId;
        }
    }
}