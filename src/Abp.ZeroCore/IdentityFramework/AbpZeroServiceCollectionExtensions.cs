using System;
using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Authorization;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Abp.Zero.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection.Extensions;

// ReSharper disable once CheckNamespace - This is done to add extension methods to Microsoft.Extensions.DependencyInjection namespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class AbpZeroServiceCollectionExtensions
    {
        public static AbpIdentityBuilder AddAbpIdentity<TTenant, TUser, TRole>(this IServiceCollection services)
            where TTenant : AbpTenant<TUser>
            where TRole : AbpRole<TUser>, new()
            where TUser : AbpUser<TUser>
        {
            return services.AddAbpIdentity<TTenant, TUser, TRole>(setupAction: null);
        }

        public static AbpIdentityBuilder AddAbpIdentity<TTenant, TUser, TRole>(this IServiceCollection services, Action<IdentityOptions> setupAction)
            where TTenant : AbpTenant<TUser>
            where TRole : AbpRole<TUser>, new()
            where TUser : AbpUser<TUser>
        {
            services.AddSingleton<IAbpZeroEntityTypes>(new AbpZeroEntityTypes
            {
                Tenant = typeof(TTenant),
                Role = typeof(TRole),
                User = typeof(TUser)
            });

            //AbpTenantManager
            services.TryAddScoped<AbpTenantManager<TTenant, TUser>>();

            //AbpEditionManager
            services.TryAddScoped<AbpEditionManager>();

            //AbpRoleManager
            services.TryAddScoped<AbpRoleManager<TRole, TUser>>();
            services.TryAddScoped(typeof(RoleManager<TRole>), provider => provider.GetService(typeof(AbpRoleManager<TRole, TUser>)));

            //AbpUserManager
            services.TryAddScoped<AbpUserManager<TRole, TUser>>();
            services.TryAddScoped(typeof(UserManager<TUser>), provider => provider.GetService(typeof(AbpUserManager<TRole, TUser>)));

            //SignInManager
            services.TryAddScoped<AbpSignInManager<TTenant, TRole, TUser>>();
            services.TryAddScoped(typeof(SignInManager<TUser>), provider => provider.GetService(typeof(AbpSignInManager<TTenant, TRole, TUser>)));

            //AbpLogInManager
            services.TryAddScoped<AbpLogInManager<TTenant, TRole, TUser>>();

            //AbpUserClaimsPrincipalFactory
            services.TryAddScoped<AbpUserClaimsPrincipalFactory<TUser, TRole>>();
            services.TryAddScoped(typeof(UserClaimsPrincipalFactory<TUser, TRole>), provider => provider.GetService(typeof(AbpUserClaimsPrincipalFactory<TUser, TRole>)));
            services.TryAddScoped(typeof(IUserClaimsPrincipalFactory<TUser>), provider => provider.GetService(typeof(AbpUserClaimsPrincipalFactory<TUser, TRole>)));

            //AbpSecurityStampValidator
            services.TryAddScoped<AbpSecurityStampValidator<TTenant, TRole, TUser>>();
            services.TryAddScoped(typeof(SecurityStampValidator<TUser>), provider => provider.GetService(typeof(AbpSecurityStampValidator<TTenant, TRole, TUser>)));
            services.TryAddScoped(typeof(ISecurityStampValidator), provider => provider.GetService(typeof(AbpSecurityStampValidator<TTenant, TRole, TUser>)));

            //PermissionChecker
            services.TryAddScoped<PermissionChecker<TRole, TUser>>();
            services.TryAddScoped(typeof(IPermissionChecker), provider => provider.GetService(typeof(PermissionChecker<TRole, TUser>)));

            //AbpUserStore
            services.TryAddScoped<AbpUserStore<TRole, TUser>>();
            services.TryAddScoped(typeof(IUserStore<TUser>), provider => provider.GetService(typeof(AbpUserStore<TRole, TUser>)));

            //AbpRoleStore
            services.TryAddScoped<AbpRoleStore<TRole, TUser>>();
            services.TryAddScoped(typeof(IRoleStore<TRole>), provider => provider.GetService(typeof(AbpRoleStore<TRole, TUser>)));

            //AbpFeatureValueStore
            services.TryAddScoped<AbpFeatureValueStore<TTenant, TUser>>();
            services.TryAddScoped(typeof(IFeatureValueStore), provider => provider.GetService(typeof(AbpFeatureValueStore<TTenant, TUser>)));

            return new AbpIdentityBuilder(services.AddIdentity<TUser, TRole>(setupAction), typeof(TTenant));
        }
    }
}
