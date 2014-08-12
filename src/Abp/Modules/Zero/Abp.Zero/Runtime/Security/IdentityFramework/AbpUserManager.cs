using Abp.Dependency;
using Abp.Users;
using Microsoft.AspNet.Identity;

namespace Abp.Runtime.Security.IdentityFramework
{
    public class AbpUserManager : UserManager<AbpUser, long>, ITransientDependency
    {
        public AbpUserManager(AbpUserStore store)
            : base(store)
        {

        }
    }
}