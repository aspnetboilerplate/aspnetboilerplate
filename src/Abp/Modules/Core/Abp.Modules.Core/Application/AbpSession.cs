using Abp.Application.Session;
using Abp.Dependency;
using Abp.Security.Tenants;
using Abp.Security.Users;

namespace Abp.Application
{
    /// <summary>
    /// 
    /// </summary>
    public class AbpSession : IAbpSession, ITransientDependency
    {
        public long? UserId
        {
            get { return AbpUser.CurrentUserId; }
        }

        public int? TenantId
        {
            get { return AbpTenant.CurrentTenantId; }
        }
    }
}