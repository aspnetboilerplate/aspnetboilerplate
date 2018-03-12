using System;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Routing;

namespace Abp.AspNetCore.OData.Configuration
{
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
        Action<IRouteBuilder> MapAction { get; set; }
    }
}
