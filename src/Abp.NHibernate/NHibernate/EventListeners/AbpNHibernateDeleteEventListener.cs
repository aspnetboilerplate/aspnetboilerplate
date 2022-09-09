using Abp.Collections.Extensions;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Events.Bus;
using Abp.Events.Bus.Entities;
using Abp.Runtime.Session;
using Abp.Timing;
using NHibernate.Engine;
using NHibernate.Event;
using NHibernate.Event.Default;
using NHibernate.Persister.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.NHibernate.EventListeners
{
    internal class AbpNHibernateDeleteEventListener : DefaultDeleteEventListener
    {
        public IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

        private readonly IIocManager _iocManager;
        private readonly Lazy<IAbpSession> _abpSession;
        private readonly Lazy<IGuidGenerator> _guidGenerator;
        private readonly Lazy<IEventBus> _eventBus;

        public AbpNHibernateDeleteEventListener(IIocManager iocManager)
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

        protected override void DeleteEntity(IEventSource session, object entity, EntityEntry entityEntry, bool isCascadeDeleteEnabled,
            IEntityPersister persister, ISet<object> transientEntities)
        {
            if (entity is ISoftDelete e)
            {
                if (!e.IsDeleted)
                {
                    e.IsDeleted = true;

                    if (e is IHasDeletionTime)
                    {
                        (e as IHasDeletionTime).DeletionTime = Clock.Now;
                    }

                    if (e is IDeletionAudited)
                    {
                        (e as IDeletionAudited).DeleterUserId = _abpSession.Value.UserId;
                    }

                    EntityChangeEventHelper.TriggerEntityDeletingEvent(entity);
                    EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompleted(entity);
                }
                else
                {
                    EntityChangeEventHelper.TriggerEntityUpdatingEvent(entity);
                    EntityChangeEventHelper.TriggerEntityUpdatedEventOnUowCompleted(entity);
                }

                TriggerDomainEvents(entity);
                CascadeBeforeDelete(session, persister, entity, entityEntry, transientEntities);
                CascadeAfterDelete(session, persister, entity, transientEntities);
            }
            else
            {
                EntityChangeEventHelper.TriggerEntityDeletingEvent(entity);
                EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompleted(entity);

                TriggerDomainEvents(entity);

                base.DeleteEntity(session, entity, entityEntry, isCascadeDeleteEnabled, persister, transientEntities);
            }
        }

        protected override async Task DeleteEntityAsync(IEventSource session, object entity, EntityEntry entityEntry, bool isCascadeDeleteEnabled,
            IEntityPersister persister, ISet<object> transientEntities, CancellationToken cancellationToken)
        {
            if (entity is ISoftDelete e)
            {
                if (!e.IsDeleted)
                {
                    e.IsDeleted = true;

                    if (e is IHasDeletionTime)
                    {
                        (e as IHasDeletionTime).DeletionTime = Clock.Now;
                    }

                    if (e is IDeletionAudited)
                    {
                        (e as IDeletionAudited).DeleterUserId = _abpSession.Value.UserId;
                    }

                    EntityChangeEventHelper.TriggerEntityDeletingEvent(entity);
                    EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompleted(entity);
                }
                else
                {
                    EntityChangeEventHelper.TriggerEntityUpdatingEvent(entity);
                    EntityChangeEventHelper.TriggerEntityUpdatedEventOnUowCompleted(entity);
                }

                TriggerDomainEvents(entity);
                await CascadeBeforeDeleteAsync(session, persister, entity, entityEntry, transientEntities, cancellationToken);
                await CascadeAfterDeleteAsync(session, persister, entity, transientEntities, cancellationToken);
            }
            else
            {
                EntityChangeEventHelper.TriggerEntityDeletingEvent(entity);
                EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompleted(entity);

                TriggerDomainEvents(entity);

                await base.DeleteEntityAsync(session, entity, entityEntry, isCascadeDeleteEnabled, persister, transientEntities, cancellationToken);
            }
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
    }
}
