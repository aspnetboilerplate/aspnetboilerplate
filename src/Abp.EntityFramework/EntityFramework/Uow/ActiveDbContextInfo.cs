using System.Data.Entity;

namespace Abp.EntityFramework.Uow
{
    public class ActiveDbContextInfo
    {
        public DbContext DbContext { get; }

        public ActiveDbContextInfo(DbContext dbContext)
        {
            DbContext = dbContext;
        }
    }
}