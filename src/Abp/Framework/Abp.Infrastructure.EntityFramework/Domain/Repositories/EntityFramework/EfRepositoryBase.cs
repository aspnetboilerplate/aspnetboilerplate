using Abp.Domain.Entities;

namespace Abp.Domain.Repositories.EntityFramework
{
    /// <summary>
    /// A shortcut of <see cref="EfRepositoryBase{TEntity,TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public class EfRepositoryBase<TEntity> : EfRepositoryBase<TEntity, int>, IRepository<TEntity> where TEntity : class, IEntity<int>, new()
    {

    }
}