using System.Data.Entity;
using System.Threading.Tasks;
using Abp.MultiTenancy;

namespace Abp.EntityFramework
{
    public sealed class SimpleDbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
        where TDbContext : DbContext
    {
        public TDbContext DbContext { get; }

        public SimpleDbContextProvider(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public TDbContext GetDbContext()
        {
            return DbContext;
        }

        public TDbContext GetDbContext(MultiTenancySides? multiTenancySide)
        {
            return DbContext;
        }

        public Task<TDbContext> GetDbContextAsync()
        {
            return Task.FromResult(DbContext);
        }

        public Task<TDbContext> GetDbContextAsync(MultiTenancySides? multiTenancySide)
        {
            return Task.FromResult(DbContext);
        }
    }
}
