using MyAbpZeroProject.EntityFramework;

namespace MyAbpZeroProject.Migrations.SeedData
{
    public class InitialDataBuilder
    {
        private readonly MyAbpZeroProjectDbContext _context;

        public InitialDataBuilder(MyAbpZeroProjectDbContext context)
        {
            _context = context;
        }

        public void Build()
        {
            new DefaultTenantRoleAndUserBuilder(_context).Build();
        }
    }
}
