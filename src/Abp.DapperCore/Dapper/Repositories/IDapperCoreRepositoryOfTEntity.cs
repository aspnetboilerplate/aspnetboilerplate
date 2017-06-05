using Abp.Domain.Entities;

namespace Abp.DapperCore.Repositories
{
    public interface IDapperCoreRepository<TEntity> : IDapperCoreRepository<TEntity, int> where TEntity : class, IEntity<int>
    {
    }
}
