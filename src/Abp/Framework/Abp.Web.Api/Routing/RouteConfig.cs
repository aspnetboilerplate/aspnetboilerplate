using System.Net.Http.Formatting;
using System.Web.Http;
using Newtonsoft.Json.Serialization;

namespace Abp.WebApi.Routing
{
    public static class RouteConfig 
    {
        public static void Register(HttpConfiguration config)
        {
            //Dynamic Web APIs
            config.Routes.MapHttpRoute(
                name: "AbpDynamicWebApi",
                routeTemplate: "api/services/{serviceName}/{action}"
                );
        }
    }
}
