using System.Linq;
using Abp.Organizations;
using Abp.Zero.SampleApp.MultiTenancy;
using NHibernate;
using NHibernate.Linq;

namespace Abp.Zero.SampleApp.NHibernate.TestDatas
{
    /* Creates OU tree for default tenant as shown below:
     * 
     * - OU1
     *   - OU11
     *     - OU111
     *     - OU112
     *   - OU12
     * - OU2
     *   - OU21
     */
    public class InitialTestOrganizationUnitsBuilder
    {
        private readonly ISession _session;
        private Tenant _defaultTenant;

        public InitialTestOrganizationUnitsBuilder(ISession session)
        {
            _session = session;
        }

        public void Build()
        {
            _defaultTenant = _session.Query<Tenant>().Single(t => t.Name == Tenant.DefaultTenantName);

            CreateOUs();
        }

        private void CreateOUs()
        {
            var ou1 = CreateOU("OU1", OrganizationUnit.CreateCode(1));
            var ou11 = CreateOU("OU11", OrganizationUnit.CreateCode(1, 1), ou1.Id);
            var ou111 = CreateOU("OU111", OrganizationUnit.CreateCode(1, 1, 1), ou11.Id);
            var ou112 = CreateOU("OU112", OrganizationUnit.CreateCode(1, 1, 2), ou11.Id);
            var ou12 = CreateOU("OU12", OrganizationUnit.CreateCode(1, 2), ou1.Id);
            var ou2 = CreateOU("OU2", OrganizationUnit.CreateCode(2));
            var ou21 = CreateOU("OU21", OrganizationUnit.CreateCode(2, 1), ou2.Id);
        }

        private OrganizationUnit CreateOU(string displayName, string code, long? parentId = null)
        {
            var ou = new OrganizationUnit(_defaultTenant.Id, displayName, parentId) {Code = code};
            _session.Save(ou);
            return ou;
        }
    }
}
