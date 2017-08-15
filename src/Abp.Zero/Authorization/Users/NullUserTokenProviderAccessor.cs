using Abp.Dependency;
using Microsoft.AspNet.Identity;

namespace Abp.Authorization.Users
{
    public class NullUserTokenProviderAccessor : IUserTokenProviderAccessor, ISingletonDependency
    {
        public IUserTokenProvider<TUser, long> GetUserTokenProviderOrNull<TUser>() where TUser : AbpUser<TUser>
        {
            return null;
        }
    }
}