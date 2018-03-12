using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.AspNetCore.OData.Configuration
{
    public static class AbpAspNetCoreODataRouteBuilderExtensions
    {
        public static void MapODataServiceRoute(this IRouteBuilder routes, IApplicationBuilder app)
        {
            var configuration = app.ApplicationServices.GetService<IAbpAspNetCoreODataModuleConfiguration>();

            configuration.MapAction(routes);
        }
    }
}
