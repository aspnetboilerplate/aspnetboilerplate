using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace Abp.Web.Startup
{
    public static class RouteConfig
    { 
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "AbpServiceApi",
                routeTemplate: "api/services/{serviceName}/{methodName}"
                );

            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            config.Formatters.Clear();
            config.Formatters.Add(formatter);
        }
    }
}
