using Abp.Domain.Entities;

namespace Abp.Domain.Repositories
{
    /// <summary>
    /// A shortcut of <see cref="IAsyncRepository{TEntity,TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public interface IAsyncRepository<TEntity> : IAsyncRepository<TEntity, int> where TEntity : class, IEntity<int>
    {
    }
}
