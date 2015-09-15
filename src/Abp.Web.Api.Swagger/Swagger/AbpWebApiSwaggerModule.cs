using System.Reflection;
using Abp.Logging;
using Abp.Modules;
using Abp.Web;
using Abp.WebApi.Configuration;
using Abp.WebApi.Controllers;
using Abp.WebApi.Controllers.Dynamic;
using Abp.WebApi.Controllers.Dynamic.Formatters;
using Abp.WebApi.Controllers.Dynamic.Selectors;
using Abp.WebApi.Controllers.Filters;
using Abp.WebApi.Runtime.Caching;
using Newtonsoft.Json.Serialization;
using NJsonSchema;
using Abp.WebApi.Controllers.Dynamic.Builders;
using Abp.Application.Services;

namespace Abp.WebApi
{
  
    [DependsOn(typeof(AbpWebApiModule))]
    public class AbpWebApiSwaggerModule : AbpModule
    {


       
     
    }
}
