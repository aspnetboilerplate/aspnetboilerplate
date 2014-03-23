using Abp.Dependency;
using Abp.Security.Users;
using Microsoft.AspNet.Identity;

namespace Abp.Security.IdentityFramework
{
    public abstract class AbpUserManagerBase<TUser> :
        UserManager<TUser, int>,
        ITransientDependency
        where TUser : AbpUser
    {
        protected AbpUserManagerBase(IUserStore<TUser, int> store)
            : base(store)
        {

        }
    }
}