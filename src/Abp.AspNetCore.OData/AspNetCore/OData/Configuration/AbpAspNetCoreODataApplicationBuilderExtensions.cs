using Abp.Configuration.Startup;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Abp.AspNetCore.OData.Configuration
{
    public static class AbpAspNetCoreODataApplicationBuilderExtensions
    {
        public static void UseOData(this IApplicationBuilder app, Action<ODataConventionModelBuilder> builderAction)
        {
            var configuration = app.ApplicationServices.GetService<IAbpStartupConfiguration>();

            if (configuration.Modules.AbpAspNetCoreOData().ODataModelBuilder == null)
            {
                configuration.Modules.AbpAspNetCoreOData().ODataModelBuilder = new ODataConventionModelBuilder(app.ApplicationServices);
            }

            builderAction(configuration.Modules.AbpAspNetCoreOData().ODataModelBuilder);
        }
    }
}
