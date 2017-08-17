using Abp.MultiTenancy;

namespace Abp.ZeroCore.SampleApp.Core
{
    public class Tenant : AbpTenant<User>
    {
        protected Tenant()
        {

        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}