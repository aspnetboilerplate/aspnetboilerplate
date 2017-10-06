using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.IdentityServer4
{
    public static class AbpZeroIdentityServerBuilderEntityFrameworkCoreExtensions
    {
        public static IIdentityServerBuilder AddAbpPersistedGrants<TDbContext>(this IIdentityServerBuilder builder)
            where TDbContext : IAbpPersistedGrantDbContext
        {
            builder.Services.AddTransient<IPersistedGrantStore, AbpPersistedGrantStore>();
            return builder;
        }
    }
}
