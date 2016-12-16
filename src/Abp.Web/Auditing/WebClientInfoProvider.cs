using System;
using System.Net;
using System.Net.Sockets;
using System.Web;
using Castle.Core.Logging;

namespace Abp.Auditing
{
    public class WebClientInfoProvider : IClientInfoProvider
    {
        public string BrowserInfo => GetBrowserInfo();

        public string ClientIpAddress => GetClientIpAddress();

        public string ComputerName => GetComputerName();

        public ILogger Logger { get; set; }

        private readonly HttpContext _httpContext;

        /// <summary>
        /// Creates a new <see cref="WebClientInfoProvider"/>.
        /// </summary>
        public WebClientInfoProvider()
        {
            _httpContext = HttpContext.Current;
            Logger = NullLogger.Instance;
        }

        protected virtual string GetBrowserInfo()
        {
            var httpContext = HttpContext.Current ?? _httpContext;
            if (httpContext?.Request.Browser == null)
            {
                return null;
            }

            return httpContext.Request.Browser.Browser + " / " +
                   httpContext.Request.Browser.Version + " / " +
                   httpContext.Request.Browser.Platform;
        }

        protected virtual string GetClientIpAddress()
        {
            var httpContext = HttpContext.Current ?? _httpContext;
            if (httpContext?.Request.ServerVariables == null)
            {
                return null;
            }

            var clientIp = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                           httpContext.Request.ServerVariables["REMOTE_ADDR"];

            try
            {
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
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.ToString());
            }

            return clientIp;
        }

        protected virtual string GetComputerName()
        {
            var httpContext = HttpContext.Current ?? _httpContext;
            if (httpContext == null || !httpContext.Request.IsLocal)
            {
                return null;
            }

            try
            {
                var clientIp = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ??
                               httpContext.Request.ServerVariables["REMOTE_ADDR"];
                return Dns.GetHostEntry(IPAddress.Parse(clientIp)).HostName;
            }
            catch
            {
                return null;
            }
        }
    }
}
