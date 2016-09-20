using System.Linq;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.MsDependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        [CanBeNull]
        internal static T GetSingletonServiceOrNull<T>(this IServiceCollection services)
        {
            return (T)services
                .FirstOrDefault(d => d.ServiceType == typeof(T))
                ?.ImplementationInstance;
        }

        [NotNull]
        internal static T GetSingletonService<T>(this IServiceCollection services)
        {
            var service = services.GetSingletonServiceOrNull<T>();
            if (service == null)
            {
                throw new AbpException("Can not find service: " + typeof(T).AssemblyQualifiedName);
            }

            return service;
        }
    }
}
