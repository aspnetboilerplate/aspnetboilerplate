using Abp.Domain.Entities;
using Abp.Domain.Repositories.NHibernate;

namespace Taskever.Data.Repositories.NHibernate.Base
{
    public abstract class TaskeverRepositoryBase<TEntity, TPrimaryKey> : NhRepositoryBase<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {

    }
}