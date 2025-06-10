using System.Security.Claims;
using System.Threading.Tasks;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;

namespace Abp.Authorization;

public interface IAbpUserClaimsPrincipalFactory<TUser, TRole>
    where TRole : AbpRole<TUser>, new()
    where TUser : AbpUser<TUser>
{
    Task<ClaimsPrincipal> CreateAsync(TUser user);
}