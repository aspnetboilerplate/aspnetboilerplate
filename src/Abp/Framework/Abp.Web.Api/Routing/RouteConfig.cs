using System.Web.Http;

namespace Abp.WebApi.Routing
{
    public static class RouteConfig 
    {
        public static void Register(HttpConfiguration config)
        {
            //Dynamic Web APIs (with area name)
            config.Routes.MapHttpRoute(
                name: "AbpDynamicWebApi",
                routeTemplate: "api/services/{areaName}/{serviceName}/{action}"
                );
        }
    }
}
