using Abp.Domain.Entities;

namespace Abp.Domain.Repositories
{
    /// <summary>
    /// A shortcut of <see cref="IRepository{TEntity,TPrimaryKey}"/> for most used primary key type (Int32).
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : IEntity<int>
    {

    }
}
