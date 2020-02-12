using System;
using Abp.Configuration.Startup;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;

namespace Abp.WebApi.OData.Configuration
{
    internal class AbpWebApiODataModuleConfiguration : IAbpWebApiODataModuleConfiguration
    {
        public ODataConventionModelBuilder ODataModelBuilder { get; set; }

        public Action<IAbpStartupConfiguration> MapAction { get; set; }

        public AbpWebApiODataModuleConfiguration()
        {
            ODataModelBuilder = new ODataConventionModelBuilder();

            MapAction = configuration =>
            {
                configuration.Modules.AbpWebApi().HttpConfiguration.MapODataServiceRoute(
                    routeName: "ODataRoute",
                    routePrefix: "odata",
                    model: configuration.Modules.AbpWebApiOData().ODataModelBuilder.GetEdmModel()
                );
            };
        }
    }
}