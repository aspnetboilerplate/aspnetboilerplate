using System;
using Abp.AspNetCore.Configuration;
using Abp.Configuration.Startup;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;

namespace Abp.AspNetCore.OData.Configuration
{
    internal class AbpAspNetCoreODataModuleConfiguration : IAbpAspNetCoreODataModuleConfiguration
    {
        public ODataConventionModelBuilder ODataModelBuilder { get; set; }

        public Action<IAbpStartupConfiguration> MapAction { get; set; }

        public AbpAspNetCoreODataModuleConfiguration()
        {
            MapAction = configuration =>
            {
                configuration.Modules.AbpAspNetCore().RouteBuilder.MapODataServiceRoute(
                    routeName: "ODataRoute",
                    routePrefix: "odata",
                    model: configuration.Modules.AbpAspNetCoreOData().ODataModelBuilder.GetEdmModel()
                );
            };
        }
    }
}