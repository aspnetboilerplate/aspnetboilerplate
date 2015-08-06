using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Abp.Modules;
using Abp.Web;
using Abp.WebApi.Configuration;
using Abp.WebApi.Controllers;
using Abp.WebApi.Controllers.Dynamic;
using Abp.WebApi.Controllers.Dynamic.Formatters;
using Abp.WebApi.Controllers.Dynamic.Selectors;
using Abp.WebApi.Controllers.Filters;
using Newtonsoft.Json.Serialization;

namespace Abp.WebApi
{
    /// <summary>
    /// This module provides Abp features for ASP.NET Web API.
    /// </summary>
    [DependsOn(typeof(AbpWebModule))]
    public class AbpWebApiModule : AbpModule
    {
        /// <inheritdoc/>
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new ApiControllerConventionalRegistrar());
            IocManager.Register<IAbpWebApiModuleConfiguration, AbpWebApiModuleConfiguration>();
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            var httpConfiguration = IocManager.Resolve<IAbpWebApiModuleConfiguration>().HttpConfiguration;

            InitializeAspNetServices(httpConfiguration);
            InitializeFilters(httpConfiguration);
            InitializeFormatters(httpConfiguration);
            InitializeRoutes(httpConfiguration);
        }

        private void InitializeAspNetServices(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Services.Replace(typeof(IHttpControllerSelector), new AbpHttpControllerSelector(httpConfiguration));
            httpConfiguration.Services.Replace(typeof(IHttpActionSelector), new AbpApiControllerActionSelector());
            httpConfiguration.Services.Replace(typeof(IHttpControllerActivator), new AbpApiControllerActivator(IocManager));
        }

        private void InitializeFilters(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Filters.Add(IocManager.Resolve<AbpExceptionFilterAttribute>());
        }

        private void InitializeFormatters(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Formatters.Clear();
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            httpConfiguration.Formatters.Add(formatter);
            httpConfiguration.Formatters.Add(new PlainTextFormatter());
        }

        private void InitializeRoutes(HttpConfiguration httpConfiguration)
        {
            DynamicApiRouteConfig.Register(httpConfiguration);
        }
    }
}
