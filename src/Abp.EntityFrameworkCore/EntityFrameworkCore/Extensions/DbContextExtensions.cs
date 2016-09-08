using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Abp.EntityFrameworkCore.Extensions
{
    internal static class DbContextExtensions
    {
        public static bool HasRelationalTransactionManager(this DbContext dbContext)
        {
            return dbContext.Database.GetService<IDbContextTransactionManager>() is IRelationalTransactionManager;
        }
    }
}