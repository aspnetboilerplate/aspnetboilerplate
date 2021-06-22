using System.Threading.Tasks;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Abp.Authorization
{
    public class AbpSecurityStampValidator<TTenant, TRole, TUser> : SecurityStampValidator<TUser>
        where TTenant : AbpTenant<TUser>
        where TRole : AbpRole<TUser>, new()
        where TUser : AbpUser<TUser>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public AbpSecurityStampValidator(
            IOptions<SecurityStampValidatorOptions> options,
            AbpSignInManager<TTenant, TRole, TUser> signInManager,
            ISystemClock systemClock,
            ILoggerFactory loggerFactory,
            IUnitOfWorkManager unitOfWorkManager)
            : base(
                options,
                signInManager,
                systemClock,
                loggerFactory)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public override async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            await _unitOfWorkManager.WithUnitOfWorkAsync(async () => { await base.ValidateAsync(context); });
        }
    }
}
