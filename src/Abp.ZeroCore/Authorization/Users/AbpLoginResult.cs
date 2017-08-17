using System.Security.Claims;
using Abp.MultiTenancy;

namespace Abp.Authorization.Users
{
    public class AbpLoginResult<TTenant, TUser>
        where TTenant : AbpTenant<TUser>
        where TUser : AbpUserBase
    {
        public AbpLoginResultType Result { get; private set; }

        public TTenant Tenant { get; private set; }

        public TUser User { get; private set; }

        public ClaimsIdentity Identity { get; private set; }

        public AbpLoginResult(AbpLoginResultType result, TTenant tenant = null, TUser user = null)
        {
            Result = result;
            Tenant = tenant;
            User = user;
        }

        public AbpLoginResult(TTenant tenant, TUser user, ClaimsIdentity identity)
            : this(AbpLoginResultType.Success, tenant)
        {
            User = user;
            Identity = identity;
        }
    }
}