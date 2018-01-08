using System;
using Abp.Configuration.Startup;
using Microsoft.AspNet.OData.Builder;

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
        Action<IAbpStartupConfiguration> MapAction { get; set; }
    }
}