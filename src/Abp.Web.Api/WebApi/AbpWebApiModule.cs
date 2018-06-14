using System;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Abp.Logging;
using Abp.Modules;
using Abp.Web;
using Abp.WebApi.Configuration;
using Abp.WebApi.Controllers;
using Abp.WebApi.Controllers.Dynamic;
using Abp.WebApi.Controllers.Dynamic.Formatters;
using Abp.WebApi.Controllers.Dynamic.Selectors;
using Abp.WebApi.Runtime.Caching;
using Castle.MicroKernel.Registration;
using Newtonsoft.Json.Serialization;
using System.Web.Http.Description;
using System.Web.Http.ModelBinding;
using Abp.Configuration.Startup;
using Abp.Json;
using Abp.WebApi.Auditing;
using Abp.WebApi.Authorization;
using Abp.WebApi.Controllers.ApiExplorer;
using Abp.WebApi.Controllers.Dynamic.Binders;
using Abp.WebApi.Controllers.Dynamic.Builders;
using Abp.WebApi.ExceptionHandling;
using Abp.WebApi.Security.AntiForgery;
using Abp.WebApi.Uow;
using Abp.WebApi.Validation;

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

            IocManager.Register<IDynamicApiControllerBuilder, DynamicApiControllerBuilder>();
            IocManager.Register<IAbpWebApiConfiguration, AbpWebApiConfiguration>();

            Configuration.Settings.Providers.Add<ClearCacheSettingProvider>();

            Configuration.Modules.AbpWebApi().ResultWrappingIgnoreUrls.Add("/swagger");
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }

        public override void PostInitialize()
        {
            var httpConfiguration = IocManager.Resolve<IAbpWebApiConfiguration>().HttpConfiguration;

            InitializeAspNetServices(httpConfiguration);
            InitializeFilters(httpConfiguration);
            InitializeFormatters(httpConfiguration);
            InitializeRoutes(httpConfiguration);
            InitializeModelBinders(httpConfiguration);

            foreach (var controllerInfo in IocManager.Resolve<DynamicApiControllerManager>().GetAll())
            {
                IocManager.IocContainer.Register(
                    Component.For(controllerInfo.InterceptorType).LifestyleTransient(),
                    Component.For(controllerInfo.ApiControllerType)
                        .Proxy.AdditionalInterfaces(controllerInfo.ServiceInterfaceType)
                        .Interceptors(controllerInfo.InterceptorType)
                        .LifestyleTransient()
                    );

                LogHelper.Logger.DebugFormat("Dynamic web api controller is created for type '{0}' with service name '{1}'.", controllerInfo.ServiceInterfaceType.FullName, controllerInfo.ServiceName);
            }

            Configuration.Modules.AbpWebApi().HttpConfiguration.EnsureInitialized();
        }

        private void InitializeAspNetServices(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Services.Replace(typeof(IHttpControllerSelector), new AbpHttpControllerSelector(httpConfiguration, IocManager.Resolve<DynamicApiControllerManager>()));
            httpConfiguration.Services.Replace(typeof(IHttpActionSelector), new AbpApiControllerActionSelector(IocManager.Resolve<IAbpWebApiConfiguration>()));
            httpConfiguration.Services.Replace(typeof(IHttpControllerActivator), new AbpApiControllerActivator(IocManager));
            httpConfiguration.Services.Replace(typeof(IApiExplorer), IocManager.Resolve<AbpApiExplorer>());
        }

        private void InitializeFilters(HttpConfiguration httpConfiguration)
        {
            httpConfiguration.Filters.Add(IocManager.Resolve<AbpApiAuthorizeFilter>());
            httpConfiguration.Filters.Add(IocManager.Resolve<AbpAntiForgeryApiFilter>());
            httpConfiguration.Filters.Add(IocManager.Resolve<AbpApiAuditFilter>());
            httpConfiguration.Filters.Add(IocManager.Resolve<AbpApiValidationFilter>());
            httpConfiguration.Filters.Add(IocManager.Resolve<AbpApiUowFilter>());
            httpConfiguration.Filters.Add(IocManager.Resolve<AbpApiExceptionFilterAttribute>());

            httpConfiguration.MessageHandlers.Add(IocManager.Resolve<ResultWrapperHandler>());
        }

        private static void InitializeFormatters(HttpConfiguration httpConfiguration)
        {
            //Remove formatters except JsonFormatter.
            foreach (var currentFormatter in httpConfiguration.Formatters.ToList())
            {
                if (!(currentFormatter is JsonMediaTypeFormatter || 
                    currentFormatter is JQueryMvcFormUrlEncodedFormatter))
                {
                    httpConfiguration.Formatters.Remove(currentFormatter);
                }
            }

            httpConfiguration.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new AbpCamelCasePropertyNamesContractResolver();
            httpConfiguration.Formatters.Add(new PlainTextFormatter());
        }

        private static void InitializeRoutes(HttpConfiguration httpConfiguration)
        {
            //Dynamic Web APIs

            httpConfiguration.Routes.MapHttpRoute(
                name: "AbpDynamicWebApi",
                routeTemplate: "api/services/{*serviceNameWithAction}"
                );

            //Other routes

            httpConfiguration.Routes.MapHttpRoute(
                name: "AbpCacheController_Clear",
                routeTemplate: "api/AbpCache/Clear",
                defaults: new { controller = "AbpCache", action = "Clear" }
                );

            httpConfiguration.Routes.MapHttpRoute(
                name: "AbpCacheController_ClearAll",
                routeTemplate: "api/AbpCache/ClearAll",
                defaults: new { controller = "AbpCache", action = "ClearAll" }
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
