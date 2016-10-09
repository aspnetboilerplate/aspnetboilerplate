using Abp.Domain.Uow;

namespace Abp.EntityFramework
{
    public class DbContextTypeMatcher : DbContextTypeMatcher<AbpDbContext>
    {
        public DbContextTypeMatcher(ICurrentUnitOfWorkProvider currentUnitOfWorkProvider) 
            : base(currentUnitOfWorkProvider)
        {
        }
    }
}