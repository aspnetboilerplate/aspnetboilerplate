using System;
using System.Web.Http;
using Abp.WebApi.Swagger.Application;

namespace Abp.WebApi.Swagger.Configuration
{
    /// <summary>
    ///  Used to configure ABP swagger module.
    /// </summary>
    public interface IAbpSwaggerModuleConfiguration
    {
        /// <summary>
        /// Gets/sets <see cref="HttpConfiguration"/>.
        /// </summary>
        HttpConfiguration HttpConfiguration { get; set; }

        /// <summary>
        /// Get or set your abp swagger ui <see cref="AbpSwaggerUiConfig"/>.
        /// </summary>
        Action<AbpSwaggerUiConfig> AbpSwaggerUiConfigure { get; set; }
    }
}
