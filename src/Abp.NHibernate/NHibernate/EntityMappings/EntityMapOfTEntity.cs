using Abp.Domain.Entities;

namespace Abp.NHibernate.EntityMappings
{
    /// <summary>
    /// A shortcut of <see cref="EntityMap{TEntity,TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    /// <typeparam name="TEntity">Entity map</typeparam>
    public abstract class EntityMap<TEntity> : EntityMap<TEntity, int> where TEntity : IEntity<int>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="tableName">Table name</param>
        protected EntityMap(string tableName)
            : base(tableName)
        {

        }
    }
}