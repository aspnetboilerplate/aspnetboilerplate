using System;
using System.IdentityModel.Tokens.Jwt;
using Abp.Authorization.Users;
using Abp.IdentityServer4;
using Abp.Runtime.Security;
using IdentityModel;
using IdentityServer4.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class AbpZeroIdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddAbpIdentityServer<TUser>(this IIdentityServerBuilder builder, Action<AbpIdentityServerOptions> optionsAction = null)
            where TUser : AbpUser<TUser>
        {
            var options = new AbpIdentityServerOptions();
            optionsAction?.Invoke(options);

            builder.AddAspNetIdentity<TUser>();

            builder.AddProfileService<AbpProfileService<TUser>>();
            builder.AddResourceOwnerValidator<AbpResourceOwnerPasswordValidator<TUser>>();

            builder.Services.Replace(ServiceDescriptor.Transient<IClaimsService, AbpClaimsService>());

            if (options.UpdateAbpClaimTypes)
            {
                AbpClaimTypes.UserId = JwtClaimTypes.Subject;
                AbpClaimTypes.UserName = JwtClaimTypes.Name;
                AbpClaimTypes.Role = JwtClaimTypes.Role;
            }

            if (options.UpdateJwtSecurityTokenHandlerDefaultInboundClaimTypeMap)
            {
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap[AbpClaimTypes.UserId] = AbpClaimTypes.UserId;
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap[AbpClaimTypes.UserName] = AbpClaimTypes.UserName;
                JwtSecurityTokenHandler.DefaultInboundClaimTypeMap[AbpClaimTypes.Role] = AbpClaimTypes.Role;
            }

            return builder;
        }
    }
}
