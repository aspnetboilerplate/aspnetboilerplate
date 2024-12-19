using System;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OData.ModelBuilder;

namespace Abp.AspNetCore.OData.Configuration;

/// <summary>
/// Used to configure Abp.AspNetCore.OData module.
/// </summary>
public interface IAbpAspNetCoreODataModuleConfiguration
{
    /// <summary>
    /// Gets ODataConventionModelBuilder.
    /// </summary>
    ODataConventionModelBuilder ODataModelBuilder { get; set; }

    /// <summary>
    /// Allows overriding OData mapping.
    /// </summary>
    Action<IMvcCoreBuilder> MapAction { get; set; }
}