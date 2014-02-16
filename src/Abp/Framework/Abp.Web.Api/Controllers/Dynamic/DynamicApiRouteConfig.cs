using System.Web.Http;

namespace Abp.WebApi.Controllers.Dynamic
{
    /// <summary>
    /// Configures routes for dynamic controllers.
    /// </summary>
    public static class DynamicApiRouteConfig
    {
        /// <summary>
        /// Registers dynamic api controllers
        /// </summary>
        public static void Register()
        {
            //Dynamic Web APIs (with area name)
            GlobalConfiguration.Configuration.Routes.MapHttpRoute(
                name: "AbpDynamicWebApi",
                routeTemplate: "api/services/{areaName}/{serviceName}/{action}"
                );
        }
    }
}
