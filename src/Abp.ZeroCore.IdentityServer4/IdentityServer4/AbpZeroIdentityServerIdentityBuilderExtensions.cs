using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.IdentityServer4
{
    public static class AbpZeroIdentityServerIdentityBuilderExtensions
    {
        public static IdentityBuilder AddAbpIdentityServer(this IdentityBuilder builder)
        {
            builder.Services.AddIdentityServer();
            return builder;
        }
    }
}
