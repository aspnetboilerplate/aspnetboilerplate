using Abp.Data;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Transactions;

namespace Abp.Dapper.Repositories
{
    public class DapperEfRepositoryBase<TDbContext, TEntity> : DapperEfRepositoryBase<TDbContext, TEntity, int>, IDapperRepository<TEntity>
        where TEntity : class, IEntity<int>
        where TDbContext : class

    {
        public DapperEfRepositoryBase(IActiveTransactionProvider activeTransactionProvider,
            ICurrentUnitOfWorkProvider currentUnitOfWorkProvider)
            : base(activeTransactionProvider, currentUnitOfWorkProvider)
        {
        }
    }
}
