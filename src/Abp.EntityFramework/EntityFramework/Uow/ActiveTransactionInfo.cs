using System.Collections.Generic;
using System.Data.Entity;

namespace Abp.EntityFramework.Uow
{
    public class ActiveTransactionInfo
    {
        public DbContextTransaction DbContextTransaction { get; }

        public DbContext StarterDbContext { get; }

        public List<DbContext> AttendeDbContexts { get; }

        public ActiveTransactionInfo(DbContextTransaction dbContextTransaction, DbContext starterDbContext)
        {
            DbContextTransaction = dbContextTransaction;
            StarterDbContext = starterDbContext;

            AttendeDbContexts = new List<DbContext>();
        }
    }
}