using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace Abp.WebApi.Routing
{
    public static class RouteConfig 
    {
        public static void Register(HttpConfiguration config)
        {
            //Dynamic Web APIs (with area name)
            config.Routes.MapHttpRoute(
                name: "AbpDynamicWebApiWithAreaName",
                routeTemplate: "api/services/{areaName}/{serviceName}/{action}"
                );

            //Dynamic Web APIs (without area name)
            config.Routes.MapHttpRoute(
                name: "AbpDynamicWebApiWithoutAreaName",
                routeTemplate: "api/services/{serviceName}/{action}"
                );
        }
    }
}
