using Abp.Application.Session;
using Abp.Dependency;
using Abp.Security.Tenants;
using Abp.Security.Users;

namespace Abp.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class AbpSession : IAbpSession, ITransientDependency
    {
        public int? UserId
        {
            get { return AbpUser.CurrentUserId; }
        }

        public int? TenantId
        {
            get { return AbpTenant.CurrentTenantId; }
        }
    }
}