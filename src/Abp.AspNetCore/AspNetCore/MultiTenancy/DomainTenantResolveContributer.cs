using Abp.AspNetCore.Configuration;
using Abp.Dependency;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Text;
using Abp.Web.MultiTenancy;
using Microsoft.AspNetCore.Http;

namespace Abp.AspNetCore.MultiTenancy
{
    public class DomainTenantResolveContributer : ITenantResolveContributer, ITransientDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWebMultiTenancyConfiguration _multiTenancyConfiguration;

        public DomainTenantResolveContributer(
            IHttpContextAccessor httpContextAccessor, 
            IWebMultiTenancyConfiguration multiTenancyConfiguration)
        {
            _httpContextAccessor = httpContextAccessor;
            _multiTenancyConfiguration = multiTenancyConfiguration;
        }

        public int? ResolveTenantId()
        {
            if (_multiTenancyConfiguration.DomainFormat.IsNullOrEmpty())
            {
                return null;
            }

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext == null)
            {
                return null;
            }

            var hostName = httpContext.Request.Host.Host.RemovePreFix("http://", "https://");
            var result = new FormattedStringValueExtracter().Extract(hostName, _multiTenancyConfiguration.DomainFormat, true);
            if (!result.IsMatch)
            {
                return null;
            }

            var tenantIdValue = result.Matches[0].Value;
            if (tenantIdValue.IsNullOrEmpty())
            {
                return null;
            }

            int tenantId;
            return !int.TryParse(tenantIdValue, out tenantId) ? (int?)null : tenantId;
        }
    }
}