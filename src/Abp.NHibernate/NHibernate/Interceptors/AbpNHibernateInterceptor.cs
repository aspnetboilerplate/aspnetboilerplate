using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Abp.Extensions;
using Abp.Runtime.Session;
using Abp.Timing;
using NHibernate;
using NHibernate.Type;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Abp.NHibernate.Interceptors
{
    internal class AbpNHibernateInterceptor : EmptyInterceptor
    {
        public IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

        private readonly IIocManager _iocManager;
        private readonly Lazy<IAbpSession> _abpSession;
        private readonly Lazy<IGuidGenerator> _guidGenerator;
        private readonly Lazy<IEventBus> _eventBus;

        public AbpNHibernateInterceptor(IIocManager iocManager)
        {
            _iocManager = iocManager;

            _abpSession =
                new Lazy<IAbpSession>(
                    () => _iocManager.IsRegistered(typeof(IAbpSession))
                        ? _iocManager.Resolve<IAbpSession>()
                        : NullAbpSession.Instance,
                    isThreadSafe: true
                    );
            _guidGenerator =
                new Lazy<IGuidGenerator>(
                    () => _iocManager.IsRegistered(typeof(IGuidGenerator))
                        ? _iocManager.Resolve<IGuidGenerator>()
                        : SequentialGuidGenerator.Instance,
                    isThreadSafe: true
                    );

            _eventBus =
                new Lazy<IEventBus>(
                    () => _iocManager.IsRegistered(typeof(IEventBus))
                        ? _iocManager.Resolve<IEventBus>()
                        : NullEventBus.Instance,
                    isThreadSafe: true
                );
        }

        public override bool OnSave(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            //Set Id for Guids
            if (entity is IEntity<Guid>)
            {
                var guidEntity = entity as IEntity<Guid>;
                if (guidEntity.IsTransient())
                {
                    guidEntity.Id = _guidGenerator.Value.Create();
                }
            }

            //Set CreationTime for new entity
            if (entity is IHasCreationTime)
            {
                for (var i = 0; i < propertyNames.Length; i++)
                {
                    if (propertyNames[i] == "CreationTime")
                    {
                        state[i] = (entity as IHasCreationTime).CreationTime = Clock.Now;
                    }
                }
            }

            //Set CreatorUserId for new entity
            if (entity is ICreationAudited && _abpSession.Value.UserId.HasValue)
            {
                for (var i = 0; i < propertyNames.Length; i++)
                {
                    if (propertyNames[i] == "CreatorUserId")
                    {
                        state[i] = (entity as ICreationAudited).CreatorUserId = _abpSession.Value.UserId;
                    }
                }
            }

            EntityChangeEventHelper.TriggerEntityCreatingEvent(entity);
            EntityChangeEventHelper.TriggerEntityCreatedEventOnUowCompleted(entity);

            TriggerDomainEvents(entity);

            return base.OnSave(entity, id, state, propertyNames, types);
        }

        public override bool OnFlushDirty(object entity, object id, object[] currentState, object[] previousState, string[] propertyNames, IType[] types)
        {
            var updated = false;

            //Set modification audits
            if (entity is IHasModificationTime)
            {
                for (var i = 0; i < propertyNames.Length; i++)
                {
                    if (propertyNames[i] == "LastModificationTime")
                    {
                        currentState[i] = (entity as IHasModificationTime).LastModificationTime = Clock.Now;
                        updated = true;
                    }
                }
            }

            if (entity is IModificationAudited && _abpSession.Value.UserId.HasValue)
            {
                for (var i = 0; i < propertyNames.Length; i++)
                {
                    if (propertyNames[i] == "LastModifierUserId")
                    {
                        currentState[i] = (entity as IModificationAudited).LastModifierUserId = _abpSession.Value.UserId;
                        updated = true;
                    }
                }
            }

            if (entity is ISoftDelete && entity.As<ISoftDelete>().IsDeleted)
            {
                //Is deleted before? Normally, a deleted entity should not be updated later but I preferred to check it.
                var previousIsDeleted = false;
                for (var i = 0; i < propertyNames.Length; i++)
                {
                    if (propertyNames[i] == "IsDeleted")
                    {
                        previousIsDeleted = (bool)previousState[i];
                        break;
                    }
                }

                if (!previousIsDeleted)
                {
                    //set DeletionTime
                    if (entity is IHasDeletionTime)
                    {
                        for (var i = 0; i < propertyNames.Length; i++)
                        {
                            if (propertyNames[i] == "DeletionTime")
                            {
                                currentState[i] = (entity as IHasDeletionTime).DeletionTime = Clock.Now;
                                updated = true;
                            }
                        }
                    }

                    //set DeleterUserId
                    if (entity is IDeletionAudited && _abpSession.Value.UserId.HasValue)
                    {
                        for (var i = 0; i < propertyNames.Length; i++)
                        {
                            if (propertyNames[i] == "DeleterUserId")
                            {
                                currentState[i] = (entity as IDeletionAudited).DeleterUserId = _abpSession.Value.UserId;
                                updated = true;
                            }
                        }
                    }

                    EntityChangeEventHelper.TriggerEntityDeletingEvent(entity);
                    EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompleted(entity);
                }
                else
                {
                    EntityChangeEventHelper.TriggerEntityUpdatingEvent(entity);
                    EntityChangeEventHelper.TriggerEntityUpdatedEventOnUowCompleted(entity);
                }
            }
            else
            {
                EntityChangeEventHelper.TriggerEntityUpdatingEvent(entity);
                EntityChangeEventHelper.TriggerEntityUpdatedEventOnUowCompleted(entity);
            }

            TriggerDomainEvents(entity);

            return base.OnFlushDirty(entity, id, currentState, previousState, propertyNames, types) || updated;
        }

        public override void OnDelete(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            EntityChangeEventHelper.TriggerEntityDeletingEvent(entity);
            EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompleted(entity);

            TriggerDomainEvents(entity);

            base.OnDelete(entity, id, state, propertyNames, types);
        }

        protected virtual void TriggerDomainEvents(object entityAsObj)
        {
            var generatesDomainEventsEntity = entityAsObj as IGeneratesDomainEvents;
            if (generatesDomainEventsEntity == null)
            {
                return;
            }

            if (generatesDomainEventsEntity.DomainEvents.IsNullOrEmpty())
            {
                return;
            }

            var domainEvents = generatesDomainEventsEntity.DomainEvents.ToList();
            generatesDomainEventsEntity.DomainEvents.Clear();

            foreach (var domainEvent in domainEvents)
            {
                _eventBus.Value.Trigger(domainEvent.GetType(), entityAsObj, domainEvent);
            }
        }

        public override bool OnLoad(object entity, object id, object[] state, string[] propertyNames, IType[] types)
        {
            if (entity.GetType().IsDefined(typeof(DisableDateTimeNormalizationAttribute), true))
            {
                return true;
            }

            NormalizeDateTimePropertiesForEntity(entity, state, propertyNames, types);
            return true;
        }

        private static void NormalizeDateTimePropertiesForEntity(object entity, object[] state, string[] propertyNames, IList<IType> types)
        {
            for (var i = 0; i < types.Count; i++)
            {
                var prop = entity.GetType().GetProperty(propertyNames[i]);
                if (prop != null && prop.IsDefined(typeof(DisableDateTimeNormalizationAttribute), true))
                {
                    continue;
                }

                if (types[i].IsComponentType)
                {
                    NormalizeDateTimePropertiesForComponentType(state[i], types[i]);
                }

                if (types[i].ReturnedClass != typeof(DateTime) && types[i].ReturnedClass != typeof(DateTime?))
                {
                    continue;
                }

                var dateTime = state[i] as DateTime?;

                if (!dateTime.HasValue)
                {
                    continue;
                }

                state[i] = Clock.Normalize(dateTime.Value);
            }
        }

        private static void NormalizeDateTimePropertiesForComponentType(object componentObject, IType type)
        {
            if (componentObject == null)
            {
                return;
            }

            var componentType = type as ComponentType;
            if (componentType == null)
            {
                return;
            }

            for (int i = 0; i < componentType.PropertyNames.Length; i++)
            {
                var propertyName = componentType.PropertyNames[i];
                if (componentType.Subtypes[i].IsComponentType)
                {
                    var prop = componentObject.GetType().GetProperty(propertyName);
                    if (prop == null)
                    {
                        continue;
                    }

                    if (prop.IsDefined(typeof(DisableDateTimeNormalizationAttribute), true))
                    {
                        continue;
                    }

                    var value = prop.GetValue(componentObject, null);
                    NormalizeDateTimePropertiesForComponentType(value, componentType.Subtypes[i]);
                }

                if (componentType.Subtypes[i].ReturnedClass != typeof(DateTime) && componentType.Subtypes[i].ReturnedClass != typeof(DateTime?))
                {
                    continue;
                }

                var subProp = componentObject.GetType().GetProperty(propertyName);
                if (subProp == null)
                {
                    continue;
                }

                if (subProp.IsDefined(typeof(DisableDateTimeNormalizationAttribute), true))
                {
                    continue;    
                }

                var dateTime = subProp.GetValue(componentObject) as DateTime?;

                if (!dateTime.HasValue)
                {
                    continue;
                }

                subProp.SetValue(componentObject, Clock.Normalize(dateTime.Value));
            }
        }
    }
}
