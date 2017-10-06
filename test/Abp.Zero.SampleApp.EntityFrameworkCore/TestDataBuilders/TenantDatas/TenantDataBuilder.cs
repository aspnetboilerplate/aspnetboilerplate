namespace Abp.Zero.SampleApp.EntityFrameworkCore.TestDataBuilders.TenantDatas
{
    public class TenantDataBuilder
    {
        private readonly AppDbContext _context;

        public TenantDataBuilder(AppDbContext context)
        {
            _context = context;
        }

        public void Build(int tenantId)
        {
            new TenantUserBuilder(_context).Build(tenantId);
        }
    }
}