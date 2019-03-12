using Abp.Zero.SampleApp.EntityFramework;
using EntityFramework.DynamicFilters;

namespace Abp.Zero.SampleApp.Tests.TestDatas
{
    public class InitialTestDataBuilder
    {
        private readonly AppDbContext _context;

        public InitialTestDataBuilder(AppDbContext context)
        {
            _context = context;
        }

        public void Build()
        {
            _context.DisableAllFilters();

            new InitialTenantsBuilder(_context).Build();
            new InitialUsersBuilder(_context).Build();
            new InitialRolesBuilder(_context).Build();
            new InitialTestLanguagesBuilder(_context).Build();
            new InitialTestOrganizationUnitsBuilder(_context).Build();
            new InitialUserOrganizationUnitsBuilder(_context).Build();
            new InitialOrganizationUnitRolesBuilder(_context).Build();
            new InitialTestBlogBuilder(_context).Build();
        }
    }
}