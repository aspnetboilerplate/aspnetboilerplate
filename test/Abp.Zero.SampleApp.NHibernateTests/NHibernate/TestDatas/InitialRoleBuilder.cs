using System.Linq;
using Abp.Zero.SampleApp.MultiTenancy;
using NHibernate;

namespace Abp.Zero.SampleApp.NHibernate.TestDatas
{
    public class InitialRoleBuilder
    {
        private readonly ISession _session;

        public InitialRoleBuilder(ISession session)
        {
            _session = session;
        }

        public void Build()
        {
            CreateRoles();
        }

        private void CreateRoles()
        {
            var defaultTenant = _session.Query<Tenant>().Single(t => t.Name == Tenant.DefaultTenantName);

            var role = new Roles.Role
            {
                TenantId = defaultTenant.Id,
                Name = "Admin",
                DisplayName = "Administrator"
            };

            role.SetNormalizedName();

            _session.Save(role);
        }
    }
}