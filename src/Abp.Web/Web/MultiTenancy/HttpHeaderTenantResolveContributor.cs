using System.Web;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Extensions;
using Abp.MultiTenancy;
using Castle.Core.Logging;

namespace Abp.Web.MultiTenancy
{
    public class HttpHeaderTenantResolveContributor : ITenantResolveContributor, ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly IMultiTenancyConfig _multiTenancyConfig;

        public HttpHeaderTenantResolveContributor(IMultiTenancyConfig multiTenancyConfig)
        {
            _multiTenancyConfig = multiTenancyConfig;

            Logger = NullLogger.Instance;
        }

        public int? ResolveTenantId()
        {
            var httpContext = HttpContext.Current;
            if (httpContext == null)
            {
                return null;
            }

            var tenantIdHeader = httpContext.Request.Headers[_multiTenancyConfig.TenantIdResolveKey];
            if (tenantIdHeader.IsNullOrEmpty())
            {
                return null;
            }

            return int.TryParse(tenantIdHeader, out var tenantId) ? tenantId : (int?) null;
        }
    }
}
