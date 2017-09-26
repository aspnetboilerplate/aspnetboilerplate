using Abp.Zero.SampleApp.MultiTenancy;
using NHibernate;

namespace Abp.Zero.SampleApp.NHibernate.TestDatas
{
    public class InitialTenantsBuilder
    {
        private readonly ISession _session;

        public InitialTenantsBuilder(ISession session)
        {
            _session = session;
        }

        public void Build()
        {
            CreateTenants();
        }

        private void CreateTenants()
        {
            _session.Save(new Tenant(Tenant.DefaultTenantName, Tenant.DefaultTenantName));
        }
    }
}