using System.Web;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Extensions;
using Abp.MultiTenancy;

namespace Abp.Web.MultiTenancy
{
    public class HttpCookieTenantResolveContributor : ITenantResolveContributor, ITransientDependency
    {
        private readonly IMultiTenancyConfig _multiTenancyConfig;

        public HttpCookieTenantResolveContributor(IMultiTenancyConfig multiTenancyConfig)
        {
            _multiTenancyConfig = multiTenancyConfig;
        }

        public int? ResolveTenantId()
        {
            var cookie = HttpContext.Current?.Request.Cookies[_multiTenancyConfig.TenantIdResolveKey];
            if (cookie == null || cookie.Value.IsNullOrEmpty())
            {
                return null;
            }

            return int.TryParse(cookie.Value, out var tenantId) ? tenantId : (int?) null;
        }
    }
}
