using System;
using System.Web;
using Abp.Configuration.Startup;
using Abp.Dependency;
using Abp.Extensions;
using Abp.MultiTenancy;
using Castle.Core.Logging;

namespace Abp.Web.MultiTenancy
{
    public class HttpCookieTenantResolveContributor : ITenantResolveContributor, ITransientDependency
    {
        private readonly IMultiTenancyConfig _multiTenancyConfig;

        public ILogger Logger { get; set; }

        public HttpCookieTenantResolveContributor(IMultiTenancyConfig multiTenancyConfig)
        {
            _multiTenancyConfig = multiTenancyConfig;
            Logger = NullLogger.Instance;
        }

        public int? ResolveTenantId()
        {
            try
            {
                var cookie = HttpContext.Current?.Request.Cookies[_multiTenancyConfig.TenantIdResolveKey];
                if (cookie == null || cookie.Value.IsNullOrEmpty())
                {
                    return null;
                }

                return int.TryParse(cookie.Value, out var tenantId) ? tenantId : (int?)null;
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
                return null;
            }
        }
    }
}
