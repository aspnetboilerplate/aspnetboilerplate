using System;
using System.Web.Http;
using Abp.WebApi.Swagger.Application;

namespace Abp.WebApi.Swagger.Configuration
{
    internal class AbpSwaggerModuleConfiguration : IAbpSwaggerModuleConfiguration
    {
        public HttpConfiguration HttpConfiguration { get; set; }

        public Action<AbpSwaggerUiConfig> AbpSwaggerUiConfigure { get; set; }

        public AbpSwaggerModuleConfiguration()
        {
            HttpConfiguration = GlobalConfiguration.Configuration;
        }
    }
}
