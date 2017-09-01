using System.Threading.Tasks;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Abp.Authorization
{
    public class AbpSecurityStampValidator<TTenant, TRole, TUser> : SecurityStampValidator<TUser>
        where TTenant : AbpTenant<TUser>
        where TRole : AbpRole<TUser>, new()
        where TUser : AbpUser<TUser>
    {
        public AbpSecurityStampValidator(
            IOptions<IdentityOptions> options,
            AbpSignInManager<TTenant, TRole, TUser> signInManager)
            : base(
                  options, 
                  signInManager)
        {
        }

        [UnitOfWork]
        public override Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            return base.ValidateAsync(context);
        }
    }
}
