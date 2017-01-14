using System;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;
using Abp.Configuration.Startup;

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