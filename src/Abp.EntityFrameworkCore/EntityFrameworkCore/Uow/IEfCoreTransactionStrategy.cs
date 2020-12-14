using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Uow
{
    public interface IEfCoreTransactionStrategy
    {
        void InitOptions(UnitOfWorkOptions options);

        Task<DbContext> CreateDbContextAsync<TDbContext>(
            string connectionString,
            IDbContextResolver dbContextResolver) where TDbContext : DbContext;

        void Commit();

        void Dispose(IIocResolver iocResolver);

        DbContext CreateDbContext<TDbContext>(
            string connectionString,
            IDbContextResolver dbContextResolver) where TDbContext : DbContext;
    }
}
