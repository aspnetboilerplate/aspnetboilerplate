using Abp.AspNetCore.Configuration;
using Abp.Configuration.Startup;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Abp.AspNetCore.OData.Configuration
{
    public static class AbpAspNetCoreODataApplicationBuilderExtensions
    {
        public static void UseOData(this IApplicationBuilder app, Action<ODataConventionModelBuilder> builderAction)
        {
            var configuration = app.ApplicationServices.GetService<IAbpStartupConfiguration>();

            if (configuration.Modules.AbpAspNetCore().RouteBuilder == null)
            {
                configuration.Modules.AbpAspNetCore().RouteBuilder = new RouteBuilder(app, new RouteBuilder(app).Build());
            }

            if (configuration.Modules.AbpAspNetCoreOData().ODataModelBuilder == null)
            {
                configuration.Modules.AbpAspNetCoreOData().ODataModelBuilder = new ODataConventionModelBuilder(app.ApplicationServices);
            }

            builderAction(configuration.Modules.AbpAspNetCoreOData().ODataModelBuilder);

            configuration.Modules.AbpAspNetCoreOData().MapAction(configuration);

            app.UseRouter(configuration.Modules.AbpAspNetCore().RouteBuilder.Build());
        }
    }
}
