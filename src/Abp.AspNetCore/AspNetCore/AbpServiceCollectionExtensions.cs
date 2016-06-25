using System;
using Abp.AspNetCore.Mvc.Providers;
using Abp.Dependency;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

            AddContextAccessors(services);

            var abpBootstrapper = AddAbpBootstrapper(services, options.IocManager);

            return WindsorRegistrationHelper.CreateServiceProvider(abpBootstrapper.IocManager.IocContainer, services);
        }

        private static void AddContextAccessors(IServiceCollection services)
        {
            //See https://github.com/aspnet/Mvc/issues/3936 to know why we added these services.
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }

        private static AbpBootstrapper AddAbpBootstrapper(IServiceCollection services, IIocManager iocManager)
        {
            var abpBootstrapper = new AbpBootstrapper(iocManager);
            services.AddSingleton(abpBootstrapper);
            return abpBootstrapper;
        }
    }
}