using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.MsDependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        internal static T GetSingletonService<T>(this IServiceCollection services)
        {
            return (T)services
                .FirstOrDefault(d => d.ServiceType == typeof(T))
                ?.ImplementationInstance;
        }
    }
}
