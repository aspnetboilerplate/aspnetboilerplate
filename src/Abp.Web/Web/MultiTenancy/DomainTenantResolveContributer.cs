using System.Web;
using Abp.Dependency;
using Abp.Extensions;
using Abp.MultiTenancy;
using Abp.Text;

namespace Abp.Web.MultiTenancy
{
    public class DomainTenantResolveContributer : ITenantResolveContributer, ITransientDependency
    {
        private readonly IWebMultiTenancyConfiguration _multiTenancyConfiguration;

        public DomainTenantResolveContributer(
            IWebMultiTenancyConfiguration multiTenancyConfiguration)
        {
            _multiTenancyConfiguration = multiTenancyConfiguration;
        }

        public int? ResolveTenantId()
        {
            if (_multiTenancyConfiguration.DomainFormat.IsNullOrEmpty())
            {
                return null;
            }

            var httpContext = HttpContext.Current;
            if (httpContext == null)
            {
                return null;
            }

            var hostName = httpContext.Request.Url.Host.RemovePreFix("http://", "https://");
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