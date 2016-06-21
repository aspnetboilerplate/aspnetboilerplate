using System;
using Castle.Windsor.MsDependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Abp.AspNetCore
{
    public static class AbpServiceCollectionExtensions
    {
        public static IServiceProvider AddAbp(this IServiceCollection services)
        {
            return services.AddAbp(bs => { });
        }

        public static IServiceProvider AddAbp(this IServiceCollection services, Action<AbpBootstrapper> bootstrapAction)
        {
            AddContextAccessors(services);

            var abpBootstrapper = AddAbpBootstrapper(services);
            bootstrapAction(abpBootstrapper);

            return WindsorRegistrationHelper.CreateServiceProvider(abpBootstrapper.IocManager.IocContainer, services);
        }

        private static void AddContextAccessors(IServiceCollection services)
        {
            //See https://github.com/aspnet/Mvc/issues/3936 to know why we added these services.
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IActionContextAccessor, ActionContextAccessor>();
        }

        private static AbpBootstrapper AddAbpBootstrapper(IServiceCollection services)
        {
            var abpBootstrapper = new AbpBootstrapper();
            services.AddSingleton(abpBootstrapper);
            return abpBootstrapper;
        }
    }
}