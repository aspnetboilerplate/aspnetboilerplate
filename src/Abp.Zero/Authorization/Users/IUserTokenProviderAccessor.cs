using Microsoft.AspNet.Identity;

namespace Abp.Authorization.Users
{
    public interface IUserTokenProviderAccessor
    {
        IUserTokenProvider<TUser, long> GetUserTokenProviderOrNull<TUser>() 
            where TUser : AbpUser<TUser>;
    }
}