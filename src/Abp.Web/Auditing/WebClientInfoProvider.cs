using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;
using Castle.Core.Logging;

namespace Abp.Auditing
{
    public class WebClientInfoProvider : IClientInfoProvider
    {
        public string BrowserInfo => GetBrowserInfo();

        public string ClientIpAddress => GetClientIpAddress()?.ToString();

        public string ComputerName => GetComputerName();

        public ILogger Logger { get; set; }

        /// <summary>
        /// Creates a new <see cref="WebClientInfoProvider"/>.
        /// </summary>
        public WebClientInfoProvider()
        {
            Logger = NullLogger.Instance;
        }

        protected virtual string GetBrowserInfo()
        {
            var httpContext = GetCurrentHttpContext();
            if (httpContext?.Request.Browser == null)
            {
                return null;
            }

            return httpContext.Request.Browser.Browser + " / " +
                   httpContext.Request.Browser.Version + " / " +
                   httpContext.Request.Browser.Platform;
        }

        protected virtual IPAddress GetClientIpAddress()
        {
            var httpContext = GetCurrentHttpContext();
            if (httpContext?.Request.ServerVariables == null)
            {
                return null;
            }

            IPAddress clientIpAddress = null;

            try
            {
                // Header Format => X-Forwarded-For: <client>, <proxy1>, <proxy2>
                // https://developer.mozilla.org/en-US/docs/Web/HTTP/Headers/X-Forwarded-For
                // X_FORWARDED_FOR header is being prefixed with HTTP
                // https://docs.microsoft.com/en-us/previous-versions/iis/6.0-sdk/ms524602(v=vs.90)#obtaining-server-variables
                var forwardIpAddress = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]?.Split(',').FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(forwardIpAddress))
                {
                    clientIpAddress = IPEndPointHelper.Parse(forwardIpAddress).Address;
                }

                // Remote Ip Address is separated into REMOTE_ADDR & REMOTE_PORT
                var remoteIpAddress = httpContext.Request.ServerVariables["REMOTE_ADDR"];
                if (clientIpAddress == null && !string.IsNullOrWhiteSpace(remoteIpAddress))
                {
                    clientIpAddress = IPAddress.Parse(remoteIpAddress);
                }

                if (clientIpAddress == null && httpContext.Request.IsLocal)
                {
                    clientIpAddress = Dns.GetHostAddresses(Dns.GetHostName()).FirstOrDefault(address =>
                        {
                            return address.AddressFamily.HasFlag(AddressFamily.InterNetwork | AddressFamily.InterNetworkV6);
                        }
                    );
                }
            }
            catch (Exception ex)
            {
                Logger.Debug(ex.ToString());
            }

            return clientIpAddress;
        }

        protected virtual string GetComputerName()
        {
            var httpContext = GetCurrentHttpContext();
            if (httpContext == null || !httpContext.Request.IsLocal)
            {
                return null;
            }

            try
            {
                return Dns.GetHostEntry(GetClientIpAddress()).HostName;
            }
            catch
            {
                return null;
            }
        }

        public virtual HttpContextBase GetCurrentHttpContext()
        {
            return HttpContext.Current == null ? null : new HttpContextWrapper(HttpContext.Current);
        }
    }
}
