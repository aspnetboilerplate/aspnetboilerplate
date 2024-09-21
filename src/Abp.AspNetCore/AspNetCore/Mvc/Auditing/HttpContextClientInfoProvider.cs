using Abp.Auditing;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Net;

namespace Abp.AspNetCore.Mvc.Auditing;

public class HttpContextClientInfoProvider : IClientInfoProvider
{
    public string BrowserInfo => GetBrowserInfo();

    public string ClientIpAddress => GetClientIpAddress();

    public string ComputerName => GetComputerName();

    public ILogger Logger { get; set; }

    private readonly IHttpContextAccessor _httpContextAccessor;

    /// <summary>
    /// Creates a new <see cref="HttpContextClientInfoProvider"/>.
    /// </summary>
    public HttpContextClientInfoProvider(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;

        Logger = NullLogger.Instance;
    }

    protected virtual string GetBrowserInfo()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        return httpContext?.Request?.Headers?["User-Agent"];
    }

    protected virtual string GetClientIpAddress()
    {
        try
        {
            var httpContext = _httpContextAccessor.HttpContext;
            string ip = string.Empty;

            // X-Forwarded-For: client, proxy1, proxy2
            if (httpContext?.Request?.Headers != null &&
                httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
            {
                var forwardedForValue = forwardedFor.FirstOrDefault();
                if (!string.IsNullOrEmpty(forwardedForValue))
                {
                    ip = forwardedForValue.Split(',')[0].Trim();
                }
            }

            // X-Real-IP: client
            if (string.IsNullOrEmpty(ip) &&
                httpContext?.Request?.Headers != null &&
                httpContext.Request.Headers.TryGetValue("X-Real-IP", out var realIp))
            {
                ip = realIp.FirstOrDefault();
            }

            // RemoteIpAddress
            if (string.IsNullOrEmpty(ip) &&
                httpContext?.Connection?.RemoteIpAddress != null)
            {
                ip = httpContext.Connection.RemoteIpAddress.ToString();
            }

            // Check if it's a valid IP
            if (!string.IsNullOrEmpty(ip) && IPAddress.TryParse(ip, out _))
            {
                return ip;
            }
        }
        catch (Exception ex)
        {
            Logger.Warn(ex.ToString());
        }
        return null;
    }

    protected virtual string GetComputerName()
    {
        return null;
    }
}