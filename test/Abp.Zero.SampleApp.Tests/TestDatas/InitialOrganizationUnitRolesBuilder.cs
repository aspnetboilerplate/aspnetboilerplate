using System.Linq;
using Abp.Organizations;
using Abp.Zero.SampleApp.EntityFramework;
using Abp.Zero.SampleApp.MultiTenancy;

namespace Abp.Zero.SampleApp.Tests.TestDatas
{
    public class InitialOrganizationUnitRolesBuilder
    {
        private readonly AppDbContext _context;

        public InitialOrganizationUnitRolesBuilder(AppDbContext context)
        {
            _context = context;
        }

        public void Build()
        {
            AddRolesToOus();
        }

        private void AddRolesToOus()
        {
            var roles = _context.Roles.ToList();
            var role = _context.Roles.Single(r => r.Name == "test_role_1");
            var defaultTenant = _context.Tenants.Single(t => t.TenancyName == Tenant.DefaultTenantName);

            var ou11 = _context.OrganizationUnits.Single(ou => ou.DisplayName == "OU11");
            var ou21 = _context.OrganizationUnits.Single(ou => ou.DisplayName == "OU21");

            _context.OrganizationUnitRoles.Add(new OrganizationUnitRole(defaultTenant.Id, role.Id, ou11.Id));
            _context.OrganizationUnitRoles.Add(new OrganizationUnitRole(defaultTenant.Id, role.Id, ou21.Id));
        }
    }
}