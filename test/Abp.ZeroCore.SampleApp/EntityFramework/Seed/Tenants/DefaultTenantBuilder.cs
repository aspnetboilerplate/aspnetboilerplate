using System.Linq;
using Abp.ZeroCore.SampleApp.Core;

namespace Abp.ZeroCore.SampleApp.EntityFramework.Seed.Tenants
{
    public class DefaultTenantBuilder
    {
        private readonly SampleAppDbContext _context;

        public DefaultTenantBuilder(SampleAppDbContext context)
        {
            _context = context;
        }

        public void Create()
        {
            CreateDefaultTenant();
        }

        private void CreateDefaultTenant()
        {
            //Default tenant

            var defaultTenant = _context.Tenants.FirstOrDefault(t => t.TenancyName == Tenant.DefaultTenantName);
            if (defaultTenant != null)
            {
                return;
            }
            
            defaultTenant = new Tenant(Tenant.DefaultTenantName, Tenant.DefaultTenantName);

            var defaultEdition = _context.Editions.FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
            if (defaultEdition != null)
            {
                defaultTenant.EditionId = defaultEdition.Id;
            }

            _context.Tenants.Add(defaultTenant);
            _context.SaveChanges();
        }
    }
}
