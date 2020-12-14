using System.Threading.Tasks;
using Abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore
{
    public sealed class SimpleDbContextProvider<TDbContext> : IDbContextProvider<TDbContext>
        where TDbContext : DbContext
    {
        public TDbContext DbContext { get; }

        public SimpleDbContextProvider(TDbContext dbContext)
        {
            DbContext = dbContext;
        }

        public Task<TDbContext> GetDbContextAsync()
        {
            return Task.FromResult(DbContext);
        }

        public Task<TDbContext> GetDbContextAsync(MultiTenancySides? multiTenancySide)
        {
            return Task.FromResult(DbContext);
        }

        public TDbContext GetDbContext()
        {
            return DbContext;
        }

        public TDbContext GetDbContext(MultiTenancySides? multiTenancySide)
        {
            return DbContext;
        }
    }
}
