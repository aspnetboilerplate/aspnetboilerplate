using System;
using System.Net;
using System.Net.Sockets;
using System.Web;
using Abp.Dependency;
using Castle.Core.Logging;

namespace Abp.Auditing
{
    /// <summary>
    /// Implements <see cref="IAuditInfoProvider"/> to fill web specific audit informations.
    /// </summary>
    public class WebAuditInfoProvider : IAuditInfoProvider, ITransientDependency
    {
        public ILogger Logger { get; set; }

        private readonly HttpContext _httpContext;

        /// <summary>
        /// Creates a new <see cref="WebAuditInfoProvider"/>.
        /// </summary>
        public WebAuditInfoProvider()
        {
            _httpContext = HttpContext.Current;
            Logger = NullLogger.Instance;
        }

        public void Fill(AuditInfo auditInfo)
        {
            var httpContext = HttpContext.Current ?? _httpContext;
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

        private static string GetBrowserInfo(HttpContext httpContext)
        {
            return httpContext.Request.Browser.Browser + " / " +
                   httpContext.Request.Browser.Version + " / " +
                   httpContext.Request.Browser.Platform;
        }

        private static string GetClientIpAddress(HttpContext httpContext)
        {
            var clientIp = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                           httpContext.Request.ServerVariables["REMOTE_ADDR"];

            foreach (var hostAddress in Dns.GetHostAddresses(clientIp))
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    return hostAddress.ToString();
                }
            }

            foreach (var hostAddress in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (hostAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    return hostAddress.ToString();
                }
            }

            return null;
        }

        private static string GetComputerName(HttpContext httpContext)
        {
            if (!httpContext.Request.IsLocal)
            {
                return null;
            }

            try
            {
                var clientIp = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                               HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                return Dns.GetHostEntry(IPAddress.Parse(clientIp)).HostName;
            }
            catch
            {
                return null;
            }
        }
    }
}
