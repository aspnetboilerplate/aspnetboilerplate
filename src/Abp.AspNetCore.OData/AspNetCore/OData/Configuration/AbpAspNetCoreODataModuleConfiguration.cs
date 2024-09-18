using System;
using Microsoft.AspNetCore.OData;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.ModelBuilder;

namespace Abp.AspNetCore.OData.Configuration;

internal class AbpAspNetCoreODataModuleConfiguration : IAbpAspNetCoreODataModuleConfiguration
{
    public ODataConventionModelBuilder ODataModelBuilder { get; set; }

    public Action<IMvcCoreBuilder> MapAction { get; set; }

    public AbpAspNetCoreODataModuleConfiguration()
    {
        MapAction = routes =>
        {
            routes.AddOData(opt => opt.AddRouteComponents("odata", ODataModelBuilder.GetEdmModel()));
        };
    }
}