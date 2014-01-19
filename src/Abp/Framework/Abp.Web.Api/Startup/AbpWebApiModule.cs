using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Abp.Modules;
using Abp.Startup;
using Abp.WebApi.Controllers.Dynamic.Formatters;
using Abp.WebApi.Controllers.Dynamic.Selectors;
using Abp.WebApi.Controllers.Filters;
using Abp.WebApi.Dependency;
using Abp.WebApi.Dependency.Installers;
using Abp.WebApi.Dependency.Interceptors;
using Abp.WebApi.Routing;
using Castle.Core;
using Castle.Windsor.Installer;
using Newtonsoft.Json.Serialization;

namespace Abp.WebApi.Startup
{
    [AbpModule("Abp.Web.Api")]
    public class AbpWebApiModule : AbpModule
    {
        public override void PreInitialize(IAbpInitializationContext initializationContext)
        {
            base.PreInitialize(initializationContext);
            AbpApiControllerInterceptorRegisterer.Initialize(initializationContext);
        }

        public override void Initialize(IAbpInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);

            initializationContext.IocContainer.Install(FromAssembly.This());

            InitializeAspNetServices();
            InitializeFilters();
            InitializeFormatters();
            InitializeRoutes();
        }

        private static void InitializeAspNetServices()
        {
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerSelector), new AbpHttpControllerSelector(GlobalConfiguration.Configuration));
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpControllerActivator), new IocControllerActivator());
            GlobalConfiguration.Configuration.Services.Replace(typeof(IHttpActionSelector), new AbpApiControllerActionSelector());
        }

        private static void InitializeFilters()
        {
            GlobalConfiguration.Configuration.Filters.Add(new AbpExceptionFilterAttribute());
        }

        private static void InitializeFormatters()
        {
            GlobalConfiguration.Configuration.Formatters.Clear();
            var formatter = new JsonMediaTypeFormatter();
            formatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            GlobalConfiguration.Configuration.Formatters.Add(formatter);
            GlobalConfiguration.Configuration.Formatters.Add(new PlainTextFormatter());
        }

        private static void InitializeRoutes()
        {
            RouteConfig.Register(GlobalConfiguration.Configuration);
        }
    }
}
