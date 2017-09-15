using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.IdentityServer4
{
    public static class AbpZeroIdentityServerIdentityBuilderExtensions
    {
        public static IdentityBuilder AddAbpIdentityServer(this IdentityBuilder builder)
        {
            builder.AddIdentityServer();

            //AbpIdentityServerUserClaimsPrincipalFactory
            var serviceType = typeof(IUserClaimsPrincipalFactory<>).MakeGenericType(builder.UserType);
            var implementationType = typeof(AbpIdentityServerUserClaimsPrincipalFactory<,>).MakeGenericType(builder.UserType, builder.RoleType);
            builder.Services.AddScoped(serviceType, implementationType);
            
            return builder;
        }
    }
}
