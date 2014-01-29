using Abp.Dependency;
using Abp.Security.Users;
using Microsoft.AspNet.Identity;

namespace Abp.Security.Identity
{
    public abstract class AbpUserManager<TUser> :
        UserManager<TUser, int>,
        ITransientDependency
        where TUser : AbpUser
    {
        protected AbpUserManager(IUserStore<TUser, int> store)
            : base(store)
        {

        }
    }
}