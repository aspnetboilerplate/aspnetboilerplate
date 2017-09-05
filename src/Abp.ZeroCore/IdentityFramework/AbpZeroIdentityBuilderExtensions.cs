using Abp.Application.Editions;
using Abp.Application.Features;
using Abp.Authorization;
using Microsoft.AspNetCore.Identity;
using Abp.Authorization.Users;
using Abp.Authorization.Roles;
using Abp.MultiTenancy;

// ReSharper disable once CheckNamespace - This is done to add extension methods to Microsoft.Extensions.DependencyInjection namespace
namespace Microsoft.Extensions.DependencyInjection
{
    public static class AbpZeroIdentityBuilderExtensions
    {
        public static AbpIdentityBuilder AddAbpTenantManager<TTenantManager>(this AbpIdentityBuilder builder)
            where TTenantManager : class
        {
            var type = typeof(TTenantManager);
            var abpManagerType = typeof(AbpTenantManager<,>).MakeGenericType(builder.TenantType, builder.UserType);
            builder.Services.AddScoped(type, provider => provider.GetRequiredService(abpManagerType));
            builder.Services.AddScoped(abpManagerType, type);
            return builder;
        }

        public static AbpIdentityBuilder AddAbpEditionManager<TEditionManager>(this AbpIdentityBuilder builder)
            where TEditionManager : class
        {
            var type = typeof(TEditionManager);
            var abpManagerType = typeof(AbpEditionManager);
            builder.Services.AddScoped(type, provider => provider.GetRequiredService(abpManagerType));
            builder.Services.AddScoped(abpManagerType, type);
            return builder;
        }

        public static AbpIdentityBuilder AddAbpRoleManager<TRoleManager>(this AbpIdentityBuilder builder)
            where TRoleManager : class
        {
            var abpManagerType = typeof(AbpRoleManager<,>).MakeGenericType(builder.RoleType, builder.UserType);
            var managerType = typeof(RoleManager<>).MakeGenericType(builder.RoleType);
            builder.Services.AddScoped(abpManagerType, services => services.GetRequiredService(managerType));
            builder.AddRoleManager<TRoleManager>();
            return builder;
        }

        public static AbpIdentityBuilder AddAbpUserManager<TUserManager>(this AbpIdentityBuilder builder)
            where TUserManager : class
        {
            var abpManagerType = typeof(AbpUserManager<,>).MakeGenericType(builder.RoleType, builder.UserType);
            var managerType = typeof(UserManager<>).MakeGenericType(builder.UserType);
            builder.Services.AddScoped(abpManagerType, services => services.GetRequiredService(managerType));
            builder.AddUserManager<TUserManager>();
            return builder;
        }

        public static AbpIdentityBuilder AddAbpSignInManager<TSignInManager>(this AbpIdentityBuilder builder)
            where TSignInManager : class
        {
            var abpManagerType = typeof(AbpSignInManager<,,>).MakeGenericType(builder.TenantType, builder.RoleType, builder.UserType);
            var managerType = typeof(SignInManager<>).MakeGenericType(builder.UserType);
            builder.Services.AddScoped(abpManagerType, services => services.GetRequiredService(managerType));
            builder.AddSignInManager<TSignInManager>();
            return builder;
        }

        public static AbpIdentityBuilder AddAbpLogInManager<TLogInManager>(this AbpIdentityBuilder builder)
            where TLogInManager : class
        {
            var type = typeof(TLogInManager);
            var abpManagerType = typeof(AbpLogInManager<,,>).MakeGenericType(builder.TenantType, builder.RoleType, builder.UserType);
            builder.Services.AddScoped(type, provider => provider.GetService(abpManagerType));
            builder.Services.AddScoped(abpManagerType, type);
            return builder;
        }

        public static AbpIdentityBuilder AddAbpUserClaimsPrincipalFactory<TUserClaimsPrincipalFactory>(this AbpIdentityBuilder builder)
            where TUserClaimsPrincipalFactory : class
        {
            var type = typeof(TUserClaimsPrincipalFactory);
            builder.Services.AddScoped(typeof(UserClaimsPrincipalFactory<,>).MakeGenericType(builder.UserType, builder.RoleType), services => services.GetRequiredService(type));
            builder.Services.AddScoped(typeof(AbpUserClaimsPrincipalFactory<,>).MakeGenericType(builder.UserType, builder.RoleType), services => services.GetRequiredService(type));
            builder.Services.AddScoped(typeof(IUserClaimsPrincipalFactory<>).MakeGenericType(builder.UserType), services => services.GetRequiredService(type));
            builder.Services.AddScoped(type);
            return builder;
        }

        public static AbpIdentityBuilder AddAbpSecurityStampValidator<TSecurityStampValidator>(this AbpIdentityBuilder builder)
            where TSecurityStampValidator : class, ISecurityStampValidator
        {
            var type = typeof(TSecurityStampValidator);
            builder.Services.AddScoped(typeof(SecurityStampValidator<>).MakeGenericType(builder.UserType), services => services.GetRequiredService(type));
            builder.Services.AddScoped(typeof(AbpSecurityStampValidator<,,>).MakeGenericType(builder.TenantType, builder.RoleType, builder.UserType), services => services.GetRequiredService(type));
            builder.Services.AddScoped(typeof(ISecurityStampValidator), services => services.GetRequiredService(type));
            builder.Services.AddScoped(type);
            return builder;
        }

        public static AbpIdentityBuilder AddPermissionChecker<TPermissionChecker>(this AbpIdentityBuilder builder)
            where TPermissionChecker : class
        {
            var type = typeof(TPermissionChecker);
            var checkerType = typeof(PermissionChecker<,>).MakeGenericType(builder.RoleType, builder.UserType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(checkerType, provider => provider.GetService(type));
            builder.Services.AddScoped(typeof(IPermissionChecker), provider => provider.GetService(type));
            return builder;
        }

        public static AbpIdentityBuilder AddAbpUserStore<TUserStore>(this AbpIdentityBuilder builder)
            where TUserStore : class
        {
            var type = typeof(TUserStore);
            var abpStoreType = typeof(AbpUserStore<,>).MakeGenericType(builder.RoleType, builder.UserType);
            var storeType = typeof(IUserStore<>).MakeGenericType(builder.UserType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(abpStoreType, services => services.GetRequiredService(type));
            builder.Services.AddScoped(storeType, services => services.GetRequiredService(type));
            return builder;
        }

        public static AbpIdentityBuilder AddAbpRoleStore<TRoleStore>(this AbpIdentityBuilder builder)
            where TRoleStore : class
        {
            var type = typeof(TRoleStore);
            var abpStoreType = typeof(AbpRoleStore<,>).MakeGenericType(builder.RoleType, builder.UserType);
            var storeType = typeof(IRoleStore<>).MakeGenericType(builder.RoleType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(abpStoreType, services => services.GetRequiredService(type));
            builder.Services.AddScoped(storeType, services => services.GetRequiredService(type));
            return builder;
        }

        public static AbpIdentityBuilder AddFeatureValueStore<TFeatureValueStore>(this AbpIdentityBuilder builder)
            where TFeatureValueStore : class
        {
            var type = typeof(TFeatureValueStore);
            var storeType = typeof(AbpFeatureValueStore<,>).MakeGenericType(builder.TenantType, builder.UserType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(storeType, provider => provider.GetService(type));
            builder.Services.AddScoped(typeof(IFeatureValueStore), provider => provider.GetService(type));
            return builder;
        }
    }
}
