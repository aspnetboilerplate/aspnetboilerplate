using Abp.Domain.Entities;

namespace Abp.Events.Bus.Datas.Entities
{
    /// <summary>
    /// This type of event can be used to notify deletion of an Entity.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class EntityDeletedEventData<TEntity> : EntityEventData<TEntity>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="entity">The entity which is deleted</param>
        public EntityDeletedEventData(TEntity entity)
            : base(entity)
        {
        }
    }
}