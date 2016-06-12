using System;
using Abp.Auditing;
using Abp.Dependency;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;

namespace Abp.AspNetCore.Mvc.Filters
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

            //return httpContext.Request.Browser.Browser + " / " +
            //       httpContext.Request.Browser.Version + " / " +
            //       httpContext.Request.Browser.Platform;
        }

        protected virtual string GetClientIpAddress(HttpContext httpContext)
        {
            return httpContext.Connection.RemoteIpAddress.ToString();   

            //var clientIp = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
            //               httpContext.Request.ServerVariables["REMOTE_ADDR"];

            //try
            //{
            //    foreach (var hostAddress in Dns.GetHostAddresses(clientIp))
            //    {
            //        if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
            //        {
            //            return hostAddress.ToString();
            //        }
            //    }

            //    foreach (var hostAddress in Dns.GetHostAddresses(Dns.GetHostName()))
            //    {
            //        if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
            //        {
            //            return hostAddress.ToString();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Logger.Debug(ex.ToString());
            //}

            //return clientIp;
        }

        protected virtual string GetComputerName(HttpContext httpContext)
        {
            return null; //TODO: Implement!

            //if (!httpContext.Request.IsLocal)
            //{
            //    return null;
            //}

            //try
            //{
            //    var clientIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
            //                   HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            //    return Dns.GetHostEntry(IPAddress.Parse(clientIp)).HostName;
            //}
            //catch
            //{
            //    return null;
            //}
        }
    }
}
