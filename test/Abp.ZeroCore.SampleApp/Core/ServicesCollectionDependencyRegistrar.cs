using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Abp.ZeroCore.SampleApp.Core
{
    public static class ServicesCollectionDependencyRegistrar
    {
        public static void Register(ServiceCollection services)
        {
            services.AddLogging();

            services.AddAbpIdentity<Tenant, User, Role>()
                .AddAbpTenantManager<TenantManager>()
                .AddAbpEditionManager<EditionManager>()
                .AddAbpRoleManager<RoleManager>()
                .AddAbpUserManager<UserManager>()
                .AddAbpSignInManager<SignInManager>()
                .AddAbpLogInManager<LogInManager>()
                .AddAbpUserClaimsPrincipalFactory<UserClaimsPrincipalFactory>()
                .AddAbpSecurityStampValidator<SecurityStampValidator>()
                .AddPermissionChecker<PermissionChecker>()
                .AddAbpUserStore<UserStore>()
                .AddAbpRoleStore<RoleStore>()
                .AddFeatureValueStore<FeatureValueStore>()
                .AddDefaultTokenProviders();
        }
    }
}
