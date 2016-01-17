using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web;

namespace Abp.WebApi.Swagger.Builders
{
    /// <summary>
    /// Abp swagger helper
    /// </summary>
    internal class AbpSwaggerHelper
    {
        /// <summary>
        /// Get and set your ApbWebApi module and application service information
        /// </summary>
        public static AbpSwaggerModel AbpSwaggerModel { get; private set; }

        static AbpSwaggerHelper()
        {
            AbpSwaggerModel = new AbpSwaggerModel();
        }

        /// <summary>
        /// Get the path your application
        /// </summary>
        /// <returns></returns>
        public static string GetApplicationPath()
        {
            var path = HttpRuntime.AppDomainAppPath;
            if (string.IsNullOrEmpty(path))
            {
                path = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName;
            }

            return path;
        }

        /// <summary>
        /// According client request to get root url
        /// </summary>
        /// <param name="request">The HTTP request message</param>
        /// <returns></returns>
        public static string DefaultRootUrlResolver(HttpRequestMessage request)
        {
            var scheme = GetHeaderValue(request, "X-Forwarded-Proto") ?? request.RequestUri.Scheme;
            var host = GetHeaderValue(request, "X-Forwarded-Host") ?? request.RequestUri.Host;
            var port = GetHeaderValue(request, "X-Forwarded-Port") ?? request.RequestUri.Port.ToString(CultureInfo.InvariantCulture);

            var httpConfiguration = request.GetConfiguration();
            var virtualPathRoot = httpConfiguration.VirtualPathRoot.TrimEnd('/');

            return $"{scheme}://{host}:{port}{virtualPathRoot}";
        }

        private static string GetHeaderValue(HttpRequestMessage request, string headerName)
        {
            IEnumerable<string> list;
            return request.Headers.TryGetValues(headerName, out list) ? list.FirstOrDefault() : null;
        }
    }
}
