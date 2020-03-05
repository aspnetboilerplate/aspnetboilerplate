using Castle.Windsor;
using Castle.Windsor.MsDependencyInjection;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Abp.AspNetCore.Dependency
{
    public static class AbpCastleWindsorHostBuilderExtensions
    {
        /// <summary>
        /// Uses WindsorServiceProviderFactory as service provider factory with given windsorContainer
        /// </summary>
        /// <param name="hostBuilder"></param>
        /// <param name="windsorContainer"></param>
        /// <returns></returns>
        public static IHostBuilder UseCastleWindsor(this IHostBuilder hostBuilder, [NotNull]IWindsorContainer windsorContainer)
        {
            Check.NotNull(windsorContainer, nameof(windsorContainer));

            return hostBuilder
                .ConfigureServices(services =>
                {
                    services.AddSingleton(windsorContainer);
                })
                .UseServiceProviderFactory(new WindsorServiceProviderFactory());
        }
    }
}
