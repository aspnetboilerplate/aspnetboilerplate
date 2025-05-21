using Abp.Auditing;
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Extensions;
using Abp.Json;
using Abp.Reflection;
using Abp.Runtime.Session;
using JetBrains.Annotations;
using NHibernate.Event;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Transactions;

namespace Abp.EntityHistory;

[UsedImplicitly]
public class NhEntityHistoryHelper : EntityHistoryHelperBase, IEntityHistoryHelper, ITransientDependency
{
    private readonly Lazy<IEntityHistoryStore> _entityHistoryStore;
    private readonly Lazy<IAbpSession> _abpSession;
    private readonly Lazy<IClientInfoProvider> _clientInfoProvider;
    private readonly Lazy<IEntityChangeSetReasonProvider> _entityChangeSetReasonProvider;

    private static readonly ConcurrentDictionary<Guid, EntityChangeSet> EntityChangeSets = new();

    public NhEntityHistoryHelper(
        IEntityHistoryConfiguration configuration,
        IUnitOfWorkManager unitOfWorkManager,
        IIocManager iocManager)
        : base(configuration, unitOfWorkManager)
    {
        _entityHistoryStore =
            new Lazy<IEntityHistoryStore>(
                () => iocManager.IsRegistered(typeof(IEntityHistoryStore))
                    ? iocManager.Resolve<IEntityHistoryStore>()
                    : NullEntityHistoryStore.Instance,
                isThreadSafe: true
            );

        _abpSession =
            new Lazy<IAbpSession>(
                () => iocManager.IsRegistered(typeof(IAbpSession))
                    ? iocManager.Resolve<IAbpSession>()
                    : NullAbpSession.Instance,
                isThreadSafe: true
            );

        _clientInfoProvider =
            new Lazy<IClientInfoProvider>(
                () => iocManager.IsRegistered(typeof(IClientInfoProvider))
                    ? iocManager.Resolve<IClientInfoProvider>()
                    : NullClientInfoProvider.Instance,
                isThreadSafe: true
            );

        _entityChangeSetReasonProvider =
            new Lazy<IEntityChangeSetReasonProvider>(
                () => iocManager.IsRegistered(typeof(IEntityChangeSetReasonProvider))
                    ? iocManager.Resolve<IEntityChangeSetReasonProvider>()
                    : NullEntityChangeSetReasonProvider.Instance,
                isThreadSafe: true
            );
    }

    public virtual void AddEntityToChangeSet(AbstractPreDatabaseOperationEvent @event)
    {
        SetProperInstances();

        var sessionId = @event.Session.SessionId;

        var shouldSaveEntityHistory = ShouldSaveEntityHistory(@event.Entity);
        if (shouldSaveEntityHistory.HasValue && !shouldSaveEntityHistory.Value)
        {
            return;
        }

        var shouldSaveAuditedPropertiesOnly = !shouldSaveEntityHistory.HasValue;

        var propertyChanges = @event switch
        {
            PreInsertEvent insert => GetPropertyChanges(insert, shouldSaveAuditedPropertiesOnly),
            PreUpdateEvent update => GetPropertyChanges(update, shouldSaveAuditedPropertiesOnly),
            PreDeleteEvent delete => GetPropertyChanges(delete, shouldSaveAuditedPropertiesOnly),
            _ => throw new ArgumentOutOfRangeException(nameof(@event))
        };

        if (propertyChanges.Count == 0)
        {
            return;
        }

        var entityChange = CreateEntityChange(@event);

        if (entityChange == null)
        {
            return;
        }

        entityChange.PropertyChanges = propertyChanges;

        var changeSet = CreateOrGetEntityChangeSet(sessionId);
        changeSet.EntityChanges.Add(entityChange);
    }

    public virtual void SaveChangeSet(Guid sessionId)
    {
        SetProperInstances();

        if (!IsEntityHistoryEnabled)
        {
            return;
        }

        if (!EntityChangeSets.TryGetValue(sessionId, out var changeSet)) return;

        UpdateChangeSet(changeSet);

        EntityChangeSets.TryRemove(sessionId, out _);

        if (changeSet.EntityChanges.Count == 0)
        {
            return;
        }

        using var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress);
        EntityHistoryStore.Save(changeSet);
        uow.Complete();
    }

    protected virtual EntityChangeSet CreateOrGetEntityChangeSet(Guid sessionId)
    {
        var entityChangeSet = EntityChangeSets.GetOrAdd(sessionId, _ => new EntityChangeSet
        {
            Reason = EntityChangeSetReasonProvider.Reason.TruncateWithPostfix(EntityChangeSet
                .MaxReasonLength), // todo: add HTTP METHOD to the beginning of the URI string

            // Fill "who did this change"
            BrowserInfo = ClientInfoProvider.BrowserInfo.TruncateWithPostfix(EntityChangeSet.MaxBrowserInfoLength),
            ClientIpAddress = ClientInfoProvider.ClientIpAddress.TruncateWithPostfix(EntityChangeSet
                .MaxClientIpAddressLength),
            ClientName = ClientInfoProvider.ComputerName.TruncateWithPostfix(EntityChangeSet.MaxClientNameLength),
            ImpersonatorTenantId = AbpSession.ImpersonatorTenantId,
            ImpersonatorUserId = AbpSession.ImpersonatorUserId,
            TenantId = AbpSession.TenantId,
            UserId = AbpSession.UserId,
        });

        return entityChangeSet;
    }

    protected virtual bool? ShouldSaveEntityHistory(object entityEntry)
    {
        ArgumentNullException.ThrowIfNull(entityEntry);

        var typeOfEntity = ProxyHelper.GetUnproxiedType(entityEntry);
        var shouldTrackEntity = IsTypeOfTrackedEntity(typeOfEntity);
        if (shouldTrackEntity.HasValue && !shouldTrackEntity.Value)
        {
            return false;
        }

        if (!IsTypeOfEntity(typeOfEntity))
        {
            return false;
        }

        var shouldAuditEntity = IsTypeOfAuditedEntity(typeOfEntity);
        if (shouldAuditEntity.HasValue && !shouldAuditEntity.Value)
        {
            return false;
        }

        return shouldAuditEntity ?? shouldTrackEntity;
    }

    protected virtual string GetEntityId(AbstractPreDatabaseOperationEvent @event)
    {
        var id = @event.Persister.GetIdentifier(@event.Entity);
        return id.ToJsonString();
    }

    protected virtual bool ShouldSavePropertyHistory(PropertyInfo propertyInfo, bool defaultValue)
    {
        if (propertyInfo == null) // Shadow properties or if mapped directly to a field
        {
            return defaultValue;
        }

        return IsAuditedPropertyInfo(propertyInfo) ?? defaultValue;
    }

    private EntityChange CreateEntityChange(AbstractPreDatabaseOperationEvent @event)
    {
        var entityId = GetEntityId(@event);
        var entityTypeFullName = ProxyHelper.GetUnproxiedType(@event.Entity).FullName;
        var changeType = @event switch
        {
            PreInsertEvent => EntityChangeType.Created,
            PreUpdateEvent update => update.IsDeleted() ? EntityChangeType.Deleted : EntityChangeType.Updated,
            PreDeleteEvent => EntityChangeType.Deleted,
            _ => throw new ArgumentOutOfRangeException(nameof(@event))
        };

        if (entityId == null)
        {
            Logger.ErrorFormat("EntityChangeType {0} must have non-empty entity id", changeType);
            return null;
        }

        return new EntityChange
        {
            ChangeType = changeType,
            EntityEntry = @event, // [NotMapped]
            EntityTypeFullName = entityTypeFullName,
            TenantId = AbpSession.TenantId,
        };
    }

    private ICollection<EntityPropertyChange> GetPropertyChanges(PreInsertEvent @event, bool auditedPropertiesOnly)
    {
        var propertyChanges = new List<EntityPropertyChange>();

        var dirtyPropertyIndexes = @event.Persister.FindDirty(@event.State, new object[@event.State.Length],
            @event.Entity, @event.Session);

        foreach (var dirtyPropertyIndex in dirtyPropertyIndexes)
        {
            var property = @event.Persister.EntityMetamodel.Properties[dirtyPropertyIndex];

            if (property.Name == @event.Persister.IdentifierPropertyName)
            {
                continue;
            }

            var propertyIndex = @event.Persister.EntityMetamodel.GetPropertyIndex(property.Name);
            var propertyInfo = @event.Entity.GetType().GetProperty(@event.Persister.PropertyNames[propertyIndex]);

            var newValue = @event.State[propertyIndex];
            var propertyName = property.Name;

            if (@event.State[propertyIndex] != null &&
                Convert.GetTypeCode(@event.State[propertyIndex]) == TypeCode.Object)
            {
                var dirtyFieldProperty = @event.Persister.EntityMetamodel.Type.GetProperties()
                    .FirstOrDefault(p => p.Name == @event.Persister.PropertyNames[propertyIndex]);
                var dirtyFieldClassType = dirtyFieldProperty?.PropertyType;
                var identityProperty = dirtyFieldClassType?.GetProperty("Id");
                if (identityProperty != null)
                {
                    newValue = identityProperty.GetValue(@event.State[propertyIndex]);
                }
            }

            if (ShouldSavePropertyHistory(propertyInfo, !auditedPropertiesOnly))
            {
                propertyChanges.Add(
                    CreateEntityPropertyChange(
                        null,
                        newValue,
                        propertyName,
                        propertyInfo
                    )
                );
            }
        }

        return propertyChanges;
    }

    private ICollection<EntityPropertyChange> GetPropertyChanges(PreUpdateEvent @event, bool auditedPropertiesOnly)
    {
        var propertyChanges = new List<EntityPropertyChange>();

        var dirtyPropertyIndexes =
            @event.Persister.FindDirty(@event.State, @event.OldState, @event.Entity, @event.Session);

        foreach (var dirtyPropertyIndex in dirtyPropertyIndexes)
        {
            var property = @event.Persister.EntityMetamodel.Properties[dirtyPropertyIndex];

            if (property.Name == @event.Persister.IdentifierPropertyName)
            {
                continue;
            }

            var propertyIndex = @event.Persister.EntityMetamodel.GetPropertyIndex(property.Name);
            var propertyInfo = @event.Entity.GetType().GetProperty(@event.Persister.PropertyNames[propertyIndex]);

            var newValue = @event.State[propertyIndex];
            var oldValue = @event.OldState[propertyIndex];
            var propertyName = property.Name;

            if (@event.State[propertyIndex] != null &&
                Convert.GetTypeCode(@event.State[propertyIndex]) == TypeCode.Object)
            {
                var dirtyFieldProperty = @event.Persister.EntityMetamodel.Type.GetProperties()
                    .FirstOrDefault(p => p.Name == @event.Persister.PropertyNames[propertyIndex]);
                var dirtyFieldClassType = dirtyFieldProperty?.PropertyType;
                var identityProperty = dirtyFieldClassType?.GetProperty("Id");
                if (identityProperty != null)
                {
                    newValue = identityProperty.GetValue(@event.State[propertyIndex]);
                }
            }

            if (@event.OldState[propertyIndex] != null &&
                Convert.GetTypeCode(@event.OldState[propertyIndex]) == TypeCode.Object)
            {
                var dirtyFieldProperty = @event.Persister.EntityMetamodel.Type.GetProperties()
                    .FirstOrDefault(p => p.Name == @event.Persister.PropertyNames[propertyIndex]);
                var dirtyFieldClassType = dirtyFieldProperty?.PropertyType;
                var identityProperty = dirtyFieldClassType?.GetProperty("Id");
                if (identityProperty != null)
                {
                    oldValue = identityProperty.GetValue(@event.OldState[propertyIndex]);
                }
            }

            if (ShouldSavePropertyHistory(propertyInfo, !auditedPropertiesOnly))
            {
                propertyChanges.Add(
                    CreateEntityPropertyChange(
                        oldValue,
                        newValue,
                        propertyName,
                        propertyInfo
                    )
                );
            }
        }

        return propertyChanges;
    }

    private ICollection<EntityPropertyChange> GetPropertyChanges(PreDeleteEvent @event, bool auditedPropertiesOnly)
    {
        var propertyChanges = new List<EntityPropertyChange>();

        var dirtyPropertyIndexes = @event.Persister.FindDirty(new object[@event.DeletedState.Length],
            @event.DeletedState, @event.Entity, @event.Session);

        foreach (var dirtyPropertyIndex in dirtyPropertyIndexes)
        {
            var property = @event.Persister.EntityMetamodel.Properties[dirtyPropertyIndex];

            if (property.Name == @event.Persister.IdentifierPropertyName)
            {
                continue;
            }

            var propertyIndex = @event.Persister.EntityMetamodel.GetPropertyIndex(property.Name);
            var propertyInfo = @event.Entity.GetType().GetProperty(@event.Persister.PropertyNames[propertyIndex]);

            var oldValue = @event.DeletedState[propertyIndex];
            var propertyName = property.Name;

            if (@event.DeletedState[propertyIndex] != null &&
                Convert.GetTypeCode(@event.DeletedState[propertyIndex]) == TypeCode.Object)
            {
                var dirtyFieldProperty = @event.Persister.EntityMetamodel.Type.GetProperties()
                    .FirstOrDefault(p => p.Name == @event.Persister.PropertyNames[propertyIndex]);
                var dirtyFieldClassType = dirtyFieldProperty?.PropertyType;
                var identityProperty = dirtyFieldClassType?.GetProperty("Id");
                if (identityProperty != null)
                {
                    oldValue = identityProperty.GetValue(@event.DeletedState[propertyIndex]);
                }
            }

            if (ShouldSavePropertyHistory(propertyInfo, !auditedPropertiesOnly))
            {
                propertyChanges.Add(
                    CreateEntityPropertyChange(
                        oldValue,
                        null,
                        propertyName,
                        propertyInfo
                    )
                );
            }
        }

        return propertyChanges;
    }

    private EntityPropertyChange CreateEntityPropertyChange(object oldValue, object newValue, string propertyName,
        PropertyInfo propertyInfo)
    {
        var entityPropertyChange = new EntityPropertyChange()
        {
            PropertyName = propertyName.TruncateWithPostfix(EntityPropertyChange.MaxPropertyNameLength),
            PropertyTypeFullName = propertyInfo.PropertyType.FullName.TruncateWithPostfix(
                EntityPropertyChange.MaxPropertyTypeFullNameLength
            ),
            TenantId = AbpSession.TenantId
        };

        entityPropertyChange.SetNewValue(newValue?.ToJsonString());
        entityPropertyChange.SetOriginalValue(oldValue?.ToJsonString());
        return entityPropertyChange;
    }

    // <summary>
    // Updates change time, entity id, Adds foreign keys, Removes/Updates property changes after SaveChanges is called.
    // </summary>
    private void UpdateChangeSet(EntityChangeSet changeSet)
    {
        var entityChangesToRemove = new List<EntityChange>();
        foreach (var entityChange in changeSet.EntityChanges)
        {
            var entityEntry = entityChange.EntityEntry.As<AbstractPreDatabaseOperationEvent>();
            var entityEntryType = ProxyHelper.GetUnproxiedType(entityEntry.Entity);
            var isAuditedEntity = IsTypeOfAuditedEntity(entityEntryType) == true;

            /* Update change time */
            entityChange.ChangeTime = GetChangeTime(entityChange.ChangeType, entityEntry.Entity);

            /* Update entity id */
            entityChange.EntityId = GetEntityId(entityEntry);


            if (!isAuditedEntity && entityChange.PropertyChanges.Count == 0)
            {
                entityChangesToRemove.Add(entityChange);
            }
        }

        foreach (var entityChange in entityChangesToRemove)
        {
            changeSet.EntityChanges.Remove(entityChange);
        }
    }

    private void SetProperInstances()
    {
        if (AbpSession == NullAbpSession.Instance)
        {
            AbpSession = _abpSession.Value;
        }

        if (EntityHistoryStore == NullEntityHistoryStore.Instance)
        {
            EntityHistoryStore = _entityHistoryStore.Value;
        }

        if (ClientInfoProvider == NullClientInfoProvider.Instance)
        {
            ClientInfoProvider = _clientInfoProvider.Value;
        }

        if (EntityChangeSetReasonProvider == NullEntityChangeSetReasonProvider.Instance)
        {
            EntityChangeSetReasonProvider = _entityChangeSetReasonProvider.Value;
        }
    }
}