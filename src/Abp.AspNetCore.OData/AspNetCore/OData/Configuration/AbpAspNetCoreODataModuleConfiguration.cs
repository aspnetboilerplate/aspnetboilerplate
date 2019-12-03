using System;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Routing;

namespace Abp.AspNetCore.OData.Configuration
{
    internal class AbpAspNetCoreODataModuleConfiguration : IAbpAspNetCoreODataModuleConfiguration
    {
        public ODataConventionModelBuilder ODataModelBuilder { get; set; }

        public Action<IRouteBuilder> MapAction { get; set; }

        public AbpAspNetCoreODataModuleConfiguration()
        {
            MapAction = routes =>
            {
                routes.MapODataServiceRoute(
                    routeName: "ODataRoute",
                    routePrefix: "odata",
                    model: ODataModelBuilder.GetEdmModel()
                );
            };
        }
    }
}
