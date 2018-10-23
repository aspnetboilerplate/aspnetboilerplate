using System.Threading.Tasks;
using Abp.Authorization.Users;
using Abp.Domain.Uow;
using Abp.Runtime.Security;
using IdentityServer4.AspNetIdentity;
using IdentityServer4.Extensions;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Identity;

namespace Abp.IdentityServer4
{
    public class AbpProfileService<TUser> : ProfileService<TUser>
        where TUser : AbpUser<TUser>
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly UserManager<TUser> _userManager;

        public AbpProfileService(
            UserManager<TUser> userManager,
            IUserClaimsPrincipalFactory<TUser> claimsFactory,
            IUnitOfWorkManager unitOfWorkManager
        ) : base(userManager, claimsFactory)
        {
            _unitOfWorkManager = unitOfWorkManager;
            _userManager = userManager;
        }

        [UnitOfWork]
        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var tenantId = context.Subject.Identity.GetTenantId();
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                await base.GetProfileDataAsync(context);
            }
        }

        [UnitOfWork]
        public override async Task IsActiveAsync(IsActiveContext context)
        {
            var tenantId = context.Subject.Identity.GetTenantId();
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                await base.IsActiveAsync(context);

                if (!context.IsActive)
                {
                    return;
                }

                var sub = context.Subject.GetSubjectId();
                var user = await _userManager.FindByIdAsync(sub);

                context.IsActive = user != null && user.IsActive;
            }
        }
    }
}
