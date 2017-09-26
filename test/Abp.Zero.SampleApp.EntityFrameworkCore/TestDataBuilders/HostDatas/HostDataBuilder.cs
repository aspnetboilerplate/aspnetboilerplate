namespace Abp.Zero.SampleApp.EntityFrameworkCore.TestDataBuilders.HostDatas
{
    public class HostDataBuilder
    {
        private readonly AppDbContext _context;

        public HostDataBuilder(AppDbContext context)
        {
            _context = context;
        }

        public void Build()
        {
            new HostUserBuilder(_context).Build();
            new HostTenantsBuilder(_context).Build();
        }
    }
}