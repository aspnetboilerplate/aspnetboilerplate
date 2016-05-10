using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Description;
using System.Web.Http.Dispatcher;
using Abp.Configuration.Startup;
using Abp.Json;
using Abp.Logging;
using Abp.Modules;
using Abp.Web;
using Abp.Web.Api.Description;
using Abp.WebApi.Configuration;
using Abp.WebApi.Controllers;
using Abp.WebApi.Controllers.Dynamic;
using Abp.WebApi.Controllers.Dynamic.Binders;
using Abp.WebApi.Controllers.Dynamic.Formatters;
using Abp.WebApi.Controllers.Dynamic.Selectors;
using Abp.WebApi.Controllers.Filters;
using Abp.WebApi.Runtime.Caching;
using Castle.MicroKernel.Registration;
using Newtonsoft.Json.Serialization;

namespace Abp.WebApi
{
    /// <summary>
    ///     This module provides Abp features for ASP.NET Web API.
    /// </summary>
    [DependsOn(typeof(AbpWebModule))]
    public class AbpWebApiModule : AbpModule
    {
        /// <inheritdoc />
        public override void PreInitialize()
        {
            IocManager.AddConventionalRegistrar(new ApiControllerConventionalRegistrar());
            IocManager.Register<IAbpWebApiModuleConfiguration, AbpWebApiModuleConfiguration>();

            Configuration.Settings.Providers.Add<ClearCacheSettingProvider>();
        }

        /// <inheritdoc />
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());

            var httpConfiguration = IocManager.Resolve<IAbpWebApiModuleConfiguration>().HttpConfiguration;

            InitializeAspNetServices(httpConfiguration);
            InitializeFilters(httpConfiguration);
            InitializeFormatters(httpConfiguration);
            InitializeRoutes(httpConfiguration);
            InitializeModelBinders(httpConfiguration);
        }

        public override void PostInitialize()
        {
            foreach (var controllerInfo in DynamicApiControllerManager.GetAll())
            {
                IocManager.IocContainer.Register(
                    Component.For(controllerInfo.InterceptorType).LifestyleTransient(),
                    Component.For(controllerInfo.ApiControllerType)
                        .Proxy.AdditionalInterfaces(controllerInfo.ServiceInterfaceType)
                        .Interceptors(controllerInfo.InterceptorType)
                        .LifestyleTransient()
                    );

                LogHelper.Logger.DebugFormat(
                    "Dynamic web api controller is created for type '{0}' with service name '{1}'.",
                    controllerInfo.ServiceInterfaceType.FullName, controllerInfo.ServiceName);
            }

            Configuration.Modules.AbpWebApi().HttpConfiguration.EnsureInitialized();
        }

        private void InitializeAspNetServices(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Services.Replace(typeof(IHttpControllerSelector),
                new AbpHttpControllerSelector(httpConfiguration));
            httpConfiguration.Services.Replace(typeof(IHttpActionSelector), new AbpApiControllerActionSelector());
            httpConfiguration.Services.Replace(typeof(IHttpControllerActivator),
                new AbpApiControllerActivator(IocManager));
            httpConfiguration.Services.Replace(typeof(IApiExplorer), new AbpApiExplorer(httpConfiguration));
        }

        private void InitializeFilters(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.MessageHandlers.Add(IocManager.Resolve<ResultWrapperHandler>());
            httpConfiguration.Filters.Add(IocManager.Resolve<AbpExceptionFilterAttribute>());
        }

        private static void InitializeFormatters(HttpConfiguration httpConfiguration)
        {
            //Remove formatters except JsonFormatter.
            foreach (var currentFormatter in httpConfiguration.Formatters.ToList())
            {
                if (!(currentFormatter is JsonMediaTypeFormatter))
                {
                    httpConfiguration.Formatters.Remove(currentFormatter);
                }
            }

            httpConfiguration.Formatters.JsonFormatter.SerializerSettings.ContractResolver =
                new CamelCasePropertyNamesContractResolver();
            httpConfiguration.Formatters.JsonFormatter.SerializerSettings.Converters.Insert(0,
                new AbpDateTimeConverter());
            httpConfiguration.Formatters.Add(new PlainTextFormatter());
        }

        private static void InitializeRoutes(HttpConfiguration httpConfiguration)
        {
            //Dynamic Web APIs

            httpConfiguration.Routes.MapHttpRoute(
                "AbpDynamicWebApi",
                "api/services/{*serviceNameWithAction}"
                );

            //Other routes

            httpConfiguration.Routes.MapHttpRoute(
                "AbpCacheController_Clear",
                "api/AbpCache/Clear",
                new {controller = "AbpCache", action = "Clear"}
                );

            httpConfiguration.Routes.MapHttpRoute(
                "AbpCacheController_ClearAll",
                "api/AbpCache/ClearAll",
                new {controller = "AbpCache", action = "ClearAll"}
                );
        }

        private static void InitializeModelBinders(HttpConfiguration httpConfiguration)
        {
            var abpApiDateTimeBinder = new AbpApiDateTimeBinder();
            httpConfiguration.BindParameter(typeof(DateTime), abpApiDateTimeBinder);
            httpConfiguration.BindParameter(typeof(DateTime?), abpApiDateTimeBinder);
        }
    }
}