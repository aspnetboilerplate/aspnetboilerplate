using System;
using Abp.Auditing;
using Abp.Dependency;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;

namespace Abp.AspNetCore.Mvc.Auditing
{
    /// <summary>
    /// Implements <see cref="IAuditInfoProvider"/> to fill web specific audit informations.
    /// </summary>
    public class HttpContextAuditInfoProvider : IAuditInfoProvider, ITransientDependency
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ILogger Logger { get; set; }

        private readonly HttpContext _httpContext;

        /// <summary>
        /// Creates a new <see cref="HttpContextAuditInfoProvider"/>.
        /// </summary>
        public HttpContextAuditInfoProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _httpContext = httpContextAccessor.HttpContext;
            Logger = NullLogger.Instance;
        }

        public virtual void Fill(AuditInfo auditInfo)
        {
            var httpContext = _httpContextAccessor.HttpContext ?? _httpContext;
            if (httpContext == null)
            {
                return;
            }

            try
            {
                auditInfo.BrowserInfo = GetBrowserInfo(httpContext);
                auditInfo.ClientIpAddress = GetClientIpAddress(httpContext);
                auditInfo.ClientName = GetComputerName(httpContext);
            }
            catch (Exception ex)
            {
                Logger.Warn("Could not obtain web parameters for audit info.");
                Logger.Warn(ex.ToString(), ex);
            }
        }

        protected virtual string GetBrowserInfo(HttpContext httpContext)
        {
            return httpContext.Request.Headers["User-Agent"];
        }

        protected virtual string GetClientIpAddress(HttpContext httpContext)
        {
            try
            {
                return httpContext.Connection.RemoteIpAddress.ToString();
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString());
            }

            return null;
        }

        protected virtual string GetComputerName(HttpContext httpContext)
        {
            return null; //TODO: Implement!
        }
    }
}
