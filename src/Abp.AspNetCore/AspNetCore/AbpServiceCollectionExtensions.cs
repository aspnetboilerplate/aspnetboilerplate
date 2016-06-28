using System;
using Abp.Dependency;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Abp.AspNetCore.Mvc.Providers;
using Abp.MsDependencyInjection.Extensions;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Abp.AspNetCore
{
    public static class AbpServiceCollectionExtensions
    {
        public static IServiceProvider AddAbp(this IServiceCollection services)
        {
            return services.AddAbp(options => { });
        }

        public static IServiceProvider AddAbp(this IServiceCollection services, Action<AbpServiceOptions> optionsAction)
        {
            var options = new AbpServiceOptions
            {
                IocManager = IocManager.Instance
            };

            optionsAction(options);

            ConfigureMvc(services, options.IocManager);

            var abpBootstrapper = AddAbpBootstrapper(services, options.IocManager);

            return WindsorRegistrationHelper.CreateServiceProvider(abpBootstrapper.IocManager.IocContainer, services);
        }

        private static void ConfigureMvc(IServiceCollection services, IIocResolver iocResolver)
        {
            //See https://github.com/aspnet/Mvc/issues/3936 to know why we added these services.
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
            
            //Use DI to create controllers
            services.Replace(ServiceDescriptor.Transient<IControllerActivator, ServiceBasedControllerActivator>());

            //Add feature providers
            var partManager = services.GetSingletonService<ApplicationPartManager>();
            partManager.FeatureProviders.Add(new AbpAppServiceControllerFeatureProvider(iocResolver));
        }

        private static AbpBootstrapper AddAbpBootstrapper(IServiceCollection services, IIocManager iocManager)
        {
            var abpBootstrapper = new AbpBootstrapper(iocManager);
            services.AddSingleton(abpBootstrapper);
            return abpBootstrapper;
        }
    }
}