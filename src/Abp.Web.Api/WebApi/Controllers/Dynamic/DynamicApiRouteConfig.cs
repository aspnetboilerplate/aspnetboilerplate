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
        /// <param name="httpConfiguration"></param>
        public static void Register(HttpConfiguration httpConfiguration)
        {
            //Dynamic Web APIs (with area name)
            httpConfiguration.Routes.MapHttpRoute(
                name: "AbpDynamicWebApi",
                routeTemplate: "api/services/{*serviceNameWithAction}"
                );
        }
    }
}
