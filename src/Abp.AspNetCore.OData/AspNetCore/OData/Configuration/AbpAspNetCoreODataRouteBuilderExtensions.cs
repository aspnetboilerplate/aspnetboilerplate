using System;
using Abp.Configuration.Startup;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.ModelBuilder;

namespace Abp.AspNetCore.OData.Configuration;

public static class AbpAspNetCoreODataRouteBuilderExtensions
{
    public static void MapODataServiceRoute(this IMvcCoreBuilder routes, IApplicationBuilder app, Action<ODataConventionModelBuilder> builderAction)
    {
        var startupConfiguration = app.ApplicationServices.GetService<IAbpStartupConfiguration>();

        startupConfiguration.Modules.AbpAspNetCoreOData().ODataModelBuilder ??= new ODataConventionModelBuilder();

        builderAction(startupConfiguration.Modules.AbpAspNetCoreOData().ODataModelBuilder);

        startupConfiguration.Modules.AbpAspNetCoreOData().MapAction(routes);
    }
}