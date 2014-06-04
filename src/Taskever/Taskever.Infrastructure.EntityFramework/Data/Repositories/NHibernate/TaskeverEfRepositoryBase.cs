using Abp.Domain.Entities;
using Abp.Domain.Repositories.EntityFramework;

namespace Taskever.Infrastructure.EntityFramework.Data.Repositories.NHibernate
{
    public abstract class TaskeverEfRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<TaskeverDbContext, TEntity, TPrimaryKey> 
        where TEntity : class, IEntity<TPrimaryKey>
    {

    }

    public abstract class TaskeverEfRepositoryBase<TEntity> : TaskeverEfRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {

    }
}