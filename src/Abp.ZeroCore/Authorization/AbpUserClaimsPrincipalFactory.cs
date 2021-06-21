using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Runtime.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Abp.Authorization
{
    public class AbpUserClaimsPrincipalFactory<TUser, TRole> : UserClaimsPrincipalFactory<TUser, TRole>, ITransientDependency
        where TRole : AbpRole<TUser>, new()
        where TUser : AbpUser<TUser>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        
        public AbpUserClaimsPrincipalFactory(
            AbpUserManager<TRole, TUser> userManager,
            AbpRoleManager<TRole, TUser> roleManager,
            IOptions<IdentityOptions> optionsAccessor, 
            IUnitOfWorkManager unitOfWorkManager) : base(userManager, roleManager, optionsAccessor)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }
        
        public override async Task<ClaimsPrincipal> CreateAsync(TUser user)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                var principal = await base.CreateAsync(user);

                if (user.TenantId.HasValue)
                {
                    principal.Identities.First().AddClaim(new Claim(AbpClaimTypes.TenantId, user.TenantId.ToString()));
                }

                return principal;
            });
        }
    }
}
