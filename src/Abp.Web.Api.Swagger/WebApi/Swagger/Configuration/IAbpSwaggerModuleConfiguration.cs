using System;
using System.Web.Http;
using Abp.WebApi.Swagger.Application;

namespace Abp.WebApi.Swagger.Configuration
{
    public interface IAbpSwaggerModuleConfiguration
    {
        HttpConfiguration HttpConfiguration { get; set; }
        Action<AbpSwaggerUiConfig> AbpSwaggerUiConfigure { get; set; }
    }
}
