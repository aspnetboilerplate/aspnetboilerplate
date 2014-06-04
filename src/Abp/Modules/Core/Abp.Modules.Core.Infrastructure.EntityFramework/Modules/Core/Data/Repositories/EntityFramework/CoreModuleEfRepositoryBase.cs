using Abp.Domain.Entities;
using Abp.Domain.Repositories.EntityFramework;

namespace Abp.Modules.Core.Data.Repositories.EntityFramework
{
    public abstract class CoreModuleEfRepositoryBase<TEntity> : CoreModuleEfRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        
    }

    public abstract class CoreModuleEfRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<CoreModuleDbContext, TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        
    }
}