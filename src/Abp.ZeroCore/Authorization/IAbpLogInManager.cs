using System.Threading.Tasks;
using Abp.Authorization.Roles;
using Abp.Authorization.Users;
using Abp.MultiTenancy;
using Microsoft.AspNetCore.Identity;

namespace Abp.Authorization;

public interface IAbpLogInManager<TTenant, TRole, TUser>
    where TTenant : AbpTenant<TUser>
    where TRole : AbpRole<TUser>, new()
    where TUser : AbpUser<TUser>
{
    Task<AbpLoginResult<TTenant, TUser>> LoginAsync(UserLoginInfo login, string tenancyName = null);
    Task<AbpLoginResult<TTenant, TUser>> LoginAsync(string userNameOrEmailAddress, string plainPassword, string tenancyName = null, bool shouldLockout = true);
    Task SaveLoginAttemptAsync(AbpLoginResult<TTenant, TUser> loginResult, string tenancyName, string userNameOrEmailAddress);
    void SaveLoginAttempt(AbpLoginResult<TTenant, TUser> loginResult, string tenancyName, string userNameOrEmailAddress);
}