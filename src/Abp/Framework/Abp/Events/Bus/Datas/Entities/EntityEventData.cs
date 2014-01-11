using Abp.Domain.Entities;

namespace Abp.Events.Bus.Datas.Entities
{
    /// <summary>
    /// Used to store data for an event that is related to with an <see cref="IEntity"/> object.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class EntityEventData<TEntity>
        where TEntity : IEntity
    {
        /// <summary>
        /// Related entity with this event.
        /// </summary>
        public TEntity Entity { get; private set; }

        /// <summary>
        /// Creates a new <see cref="EntityEventData{TEntity}"/> object.
        /// </summary>
        /// <param name="entity">Related entity with this event</param>
        protected EntityEventData(TEntity entity)
        {
            Entity = entity;
        }
    }
}