using Abp.Data;
using Abp.Domain.Entities;

namespace Abp.DapperCore.Repositories
{
    public class DapperCoreRepositoryBase<TEntity> : DapperCoreRepositoryBase<TEntity, int>, IDapperCoreRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        public DapperCoreRepositoryBase(IActiveTransactionProvider activeTransactionProvider) : base(activeTransactionProvider)
        {
        }
    }
}
