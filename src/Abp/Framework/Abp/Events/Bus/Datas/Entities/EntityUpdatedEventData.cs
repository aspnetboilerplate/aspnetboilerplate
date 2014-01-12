using Abp.Domain.Entities;

namespace Abp.Events.Bus.Datas.Entities
{
    /// <summary>
    /// This type of event can be used to notify update of an Entity.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntityUpdatedEventData<TEntity> : EntityEventData<TEntity>
        where TEntity : IEntity
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entity">The entity which is updated</param>
        protected EntityUpdatedEventData(TEntity entity)
            : base(entity)
        {
        }
    }
}