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
            var httpRequest = GetCurrentHttpRequest();
            if (httpRequest?.Browser == null)
            {
                return null;
            }

            return httpRequest.Browser.Browser + " / " +
                   httpRequest.Browser.Version + " / " +
                   httpRequest.Browser.Platform;
        }

        protected virtual IPAddress GetClientIpAddress()
        {
            var httpRequest = GetCurrentHttpRequest();
            if (httpRequest?.ServerVariables == null)
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
                var forwardIpAddress = httpRequest.ServerVariables["HTTP_X_FORWARDED_FOR"]?.Split(',').FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(forwardIpAddress))
                {
                    clientIpAddress = IPEndPointHelper.Parse(forwardIpAddress).Address;
                }

                // Remote Ip Address is separated into REMOTE_ADDR & REMOTE_PORT
                var remoteIpAddress = httpRequest.ServerVariables["REMOTE_ADDR"];
                if (clientIpAddress == null && !string.IsNullOrWhiteSpace(remoteIpAddress))
                {
                    clientIpAddress = IPAddress.Parse(remoteIpAddress);
                }

                if (clientIpAddress == null && httpRequest.IsLocal)
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
            var httpRequest = GetCurrentHttpRequest();
            if (httpRequest == null || !httpRequest.IsLocal)
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

        public virtual HttpRequestBase GetCurrentHttpRequest()
        {
            var httpContext = HttpContext.Current == null ? null : new HttpContextWrapper(HttpContext.Current);
            try
            {
                return httpContext?.Request;
            }
            catch (HttpException ex)
            {
                /* Workaround:
                 * Accessing HttpContext.Request during Application_Start or Application_End will throw exception.
                 * This behavior is intentional from microsoft
                 * See https://stackoverflow.com/questions/2518057/request-is-not-available-in-this-context/23908099#comment2514887_2518066
                 */
                Logger.Warn("HttpContext.Request access when it is not suppose to", ex);
                return null;
            }
        }
    }
}
