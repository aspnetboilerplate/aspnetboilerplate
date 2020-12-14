using System.Data.Entity;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Uow;

namespace Abp.EntityFramework.Uow
{
    public interface IEfTransactionStrategy
    {
        void InitOptions(UnitOfWorkOptions options);

        DbContext CreateDbContext<TDbContext>(string connectionString, IDbContextResolver dbContextResolver)
            where TDbContext : DbContext;

        void Commit();

        void Dispose(IIocResolver iocResolver);
        
        Task<DbContext> CreateDbContextAsync<TDbContext>(string connectionString, IDbContextResolver dbContextResolver)
            where TDbContext : DbContext;
    }
}
