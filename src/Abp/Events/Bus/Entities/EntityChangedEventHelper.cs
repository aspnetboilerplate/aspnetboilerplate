using System;
using Abp.Dependency;
using Abp.Domain.Uow;

namespace Abp.Events.Bus.Entities
{
    /// <summary>
    /// Used to trigger entity change events.
    /// </summary>
    public class EntityChangedEventHelper : ITransientDependency, IEntityChangedEventHelper
    {
        public IEventBus EventBus { get; set; }

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public EntityChangedEventHelper(IUnitOfWorkManager unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
            EventBus = NullEventBus.Instance;
        }

        public void TriggerEntityCreatedEvent(object entity)
        {
            TriggerEntityChangeEvent(typeof(EntityCreatedEventData<>), entity);
        }

        public void TriggerEntityUpdatedEvent(object entity)
        {
            TriggerEntityChangeEvent(typeof(EntityUpdatedEventData<>), entity);
        }

        public void TriggerEntityDeletedEvent(object entity)
        {
            TriggerEntityChangeEvent(typeof(EntityDeletedEventData<>), entity);
        }

        private void TriggerEntityChangeEvent(Type genericEventType, object entity)
        {
            var entityType = entity.GetType();
            var eventType = genericEventType.MakeGenericType(entityType);

            if (_unitOfWorkManager == null || _unitOfWorkManager.Current == null)
            {
                EventBus.Trigger(eventType, (IEventData)Activator.CreateInstance(eventType, new[] { entity }));
                return;
            }

            _unitOfWorkManager.Current.Completed += (sender, args) => EventBus.Trigger(eventType, (IEventData)Activator.CreateInstance(eventType, new[] { entity }));
        }
    }
}