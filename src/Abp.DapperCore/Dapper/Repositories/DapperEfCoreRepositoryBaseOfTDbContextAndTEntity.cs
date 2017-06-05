using Abp.Data;
using Abp.Domain.Entities;
using Abp.Transactions;

namespace Abp.DapperCore.Repositories
{
    public class DapperEfCoreRepositoryBase<TDbContext, TEntity> : DapperEfCoreRepositoryBase<TDbContext, TEntity, int>, IDapperCoreRepository<TEntity>
        where TEntity : class, IEntity<int>
        where TDbContext : class

    {
        public DapperEfCoreRepositoryBase(IActiveTransactionProvider activeTransactionProvider) : base(activeTransactionProvider)
        {
        }
    }
}
