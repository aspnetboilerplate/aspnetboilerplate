using Abp.Domain.Uow;
using Abp.EntityFramework;

namespace Abp.EntityFrameworkCore
{
    public class DbContextTypeMatcher : DbContextTypeMatcher<AbpDbContext>
    {
        public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
            : base(currentUnitOfWorkProvider)
        {
        }
    }
}