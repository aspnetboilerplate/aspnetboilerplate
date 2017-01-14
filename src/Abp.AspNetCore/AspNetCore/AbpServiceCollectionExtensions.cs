using System;
using Abp.AspNetCore.Mvc;
using Abp.Dependency;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Abp.AspNetCore.Mvc.Providers;
using Abp.Json;
using Abp.Modules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Abp.AspNetCore
{
    public static class AbpServiceCollectionExtensions
    {
        /// <summary>
        /// Integrates ABP to AspNet Core.
        /// </summary>
        /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="AbpModule"/>.</typeparam>
        /// <param name="services">Services.</param>
        public static IServiceProvider AddAbp<TStartupModule>(this IServiceCollection services)
            where TStartupModule : AbpModule
        {
            return services.AddAbp<TStartupModule>(options => { });
        }

        /// <summary>
        /// Integrates ABP to AspNet Core.
        /// </summary>
        /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="AbpModule"/>.</typeparam>
        /// <param name="services">Services.</param>
        /// <param name="optionsAction">An action to get/modify options</param>
        public static IServiceProvider AddAbp<TStartupModule>(this IServiceCollection services, Action<AbpServiceOptions> optionsAction)
            where TStartupModule : AbpModule
        {
            var options = new AbpServiceOptions
            {
                IocManager = IocManager.Instance
            };

            optionsAction(options);

            ConfigureAspNetCore(services, options.IocManager);

            var abpBootstrapper = AddAbpBootstrapper<TStartupModule>(services, options.IocManager);
            abpBootstrapper.PlugInSources.AddRange(options.PlugInSources);
            
            return WindsorRegistrationHelper.CreateServiceProvider(abpBootstrapper.IocManager.IocContainer, services);
        }

        private static void ConfigureAspNetCore(IServiceCollection services, IIocResolver iocResolver)
        {
            //See https://github.com/aspnet/Mvc/issues/3936 to know why we added these services.
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            
            //Use DI to create controllers
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            //Add feature providers
            var partManager = services.GetSingletonServiceOrNull<ApplicationPartManager>();
            partManager.FeatureProviders.Add(new AbpAppServiceControllerFeatureProvider(iocResolver));

            //Configure JSON serializer
            services.Configure<MvcJsonOptions>(jsonOptions =>
            {
                jsonOptions.SerializerSettings.Converters.Insert(0, new AbpDateTimeConverter());
            });

            //Configure MVC
            services.Configure<MvcOptions>(mvcOptions =>
            {
                mvcOptions.AddAbp(services);
            });
        }

        private static AbpBootstrapper AddAbpBootstrapper<TStartupModule>(IServiceCollection services, IIocManager iocManager)
            where TStartupModule : AbpModule
        {
            var abpBootstrapper = AbpBootstrapper.Create<TStartupModule>(iocManager);
            services.AddSingleton(abpBootstrapper);
            return abpBootstrapper;
        }
    }
}