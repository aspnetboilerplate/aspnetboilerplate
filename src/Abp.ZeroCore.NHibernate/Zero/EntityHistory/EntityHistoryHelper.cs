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
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;

namespace Abp.EntityHistory;

public interface IEntityHistoryHelper
{
    void AddInsertEntityToChangeSet(PreInsertEvent @event);
    void AddUpdateEntityToChangeSet(PreUpdateEvent @event);
    void AddDeleteEntityToChangeSet(PreDeleteEvent @event);
    void SaveChangeSet(Guid sessionId);
    Task SaveChangeSetAsync(Guid sessionId);
}

public class EntityHistoryHelper : EntityHistoryHelperBase, IEntityHistoryHelper, ITransientDependency
{
    private readonly IIocManager _iocManager;
    private readonly Lazy<IEntityHistoryStore> _entityHistoryStore;
    private readonly Lazy<IAbpSession> _abpSession;
    private readonly Lazy<IClientInfoProvider> _clientInfoProvider;
    private readonly Lazy<IEntityChangeSetReasonProvider> _entityChangeSetReasonProvider;

    private static ConcurrentDictionary<Guid, EntityChangeSet> _entityChangeSets = new ConcurrentDictionary<Guid, EntityChangeSet>();

    public EntityHistoryHelper(
        IEntityHistoryConfiguration configuration,
        IUnitOfWorkManager unitOfWorkManager,
        IIocManager iocManager)
        : base(configuration, unitOfWorkManager)
    {
        _iocManager = iocManager;
        _entityHistoryStore =
            new Lazy<IEntityHistoryStore>(
                () => _iocManager.IsRegistered(typeof(IEntityHistoryStore))
                    ? _iocManager.Resolve<IEntityHistoryStore>()
                    : NullEntityHistoryStore.Instance,
                isThreadSafe: true
            );
        _abpSession =
            new Lazy<IAbpSession>(
                () => _iocManager.IsRegistered(typeof(IAbpSession))
                    ? _iocManager.Resolve<IAbpSession>()
                    : NullAbpSession.Instance,
                isThreadSafe: true
            );
        _clientInfoProvider =
            new Lazy<IClientInfoProvider>(
                () => _iocManager.IsRegistered(typeof(IClientInfoProvider))
                    ? _iocManager.Resolve<IClientInfoProvider>()
                    : NullClientInfoProvider.Instance,
                isThreadSafe: true
            );
        _entityChangeSetReasonProvider =
            new Lazy<IEntityChangeSetReasonProvider>(
                () => _iocManager.IsRegistered(typeof(IEntityChangeSetReasonProvider))
                    ? _iocManager.Resolve<IEntityChangeSetReasonProvider>()
                    : NullEntityChangeSetReasonProvider.Instance,
                isThreadSafe: true
            );
    }

    public virtual void AddInsertEntityToChangeSet(PreInsertEvent @event)
    {
        SetProperInstances();

        var sessionId = @event.Session.SessionId;

        var changeSet = CreateOrGetEntityChangeSet(sessionId);

        var shouldSaveEntityHistory = ShouldSaveEntityHistory(@event.Entity);
        if (shouldSaveEntityHistory.HasValue && !shouldSaveEntityHistory.Value)
        {
            return;
        }

        var entityChange = CreateEntityChange(@event);

        if (entityChange == null)
        {
            return;
        }

        var shouldSaveAuditedPropertiesOnly = !shouldSaveEntityHistory.HasValue;
        var propertyChanges = GetPropertyChanges(@event, shouldSaveAuditedPropertiesOnly);
        if (propertyChanges.Count == 0)
        {
            return;
        }

        entityChange.PropertyChanges = propertyChanges;
        changeSet.EntityChanges.Add(entityChange);
    }
    public virtual void AddUpdateEntityToChangeSet(PreUpdateEvent @event)
    {
        SetProperInstances();

        var sessionId = @event.Session.SessionId;

        var changeSet = CreateOrGetEntityChangeSet(sessionId);

        var shouldSaveEntityHistory = ShouldSaveEntityHistory(@event.Entity);
        if (shouldSaveEntityHistory.HasValue && !shouldSaveEntityHistory.Value)
        {
            return;
        }

        var entityChange = CreateEntityChange(@event);

        if (entityChange == null)
        {
            return;
        }

        var shouldSaveAuditedPropertiesOnly = !shouldSaveEntityHistory.HasValue;
        var propertyChanges = GetPropertyChanges(@event, shouldSaveAuditedPropertiesOnly);
        if (propertyChanges.Count == 0)
        {
            return;
        }

        entityChange.PropertyChanges = propertyChanges;
        changeSet.EntityChanges.Add(entityChange);
    }
    public virtual void AddDeleteEntityToChangeSet(PreDeleteEvent @event)
    {
        var sessionId = @event.Session.SessionId;

        var changeSet = CreateOrGetEntityChangeSet(sessionId);

        var shouldSaveEntityHistory = ShouldSaveEntityHistory(@event.Entity);
        if (shouldSaveEntityHistory.HasValue && !shouldSaveEntityHistory.Value)
        {
            return;
        }

        var entityChange = CreateEntityChange(@event);

        if (entityChange == null)
        {
            return;
        }

        var shouldSaveAuditedPropertiesOnly = !shouldSaveEntityHistory.HasValue;
        var propertyChanges = GetPropertyChanges(@event, shouldSaveAuditedPropertiesOnly);
        if (propertyChanges.Count == 0)
        {
            return;
        }

        entityChange.PropertyChanges = propertyChanges;
        changeSet.EntityChanges.Add(entityChange);
    }
    public virtual void SaveChangeSet(Guid sessionId)
    {
        SetProperInstances();

        if (!IsEntityHistoryEnabled)
        {
            return;
        }

        if (!_entityChangeSets.TryGetValue(sessionId, out var changeSet)) return;

        UpdateChangeSet(changeSet);

        _entityChangeSets.TryRemove(sessionId, out _);

        if (changeSet.EntityChanges.Count == 0)
        {
            return;
        }

        using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
        {
            EntityHistoryStore.Save(changeSet);
            uow.Complete();
        }
    }
    public virtual async Task SaveChangeSetAsync(Guid sessionId)
    {
        SetProperInstances();

        if (!IsEntityHistoryEnabled)
        {
            return;
        }

        if (!_entityChangeSets.TryGetValue(sessionId, out var changeSet)) return;

        UpdateChangeSet(changeSet);

        _entityChangeSets.TryRemove(sessionId, out _);

        if (changeSet.EntityChanges.Count == 0)
        {
            return;
        }

        using (var uow = UnitOfWorkManager.Begin(TransactionScopeOption.Suppress))
        {
            await EntityHistoryStore.SaveAsync(changeSet);
            await uow.CompleteAsync();
        }
    }
    protected virtual EntityChangeSet CreateOrGetEntityChangeSet(Guid sessionId)
    {
        var entityChangeSet = _entityChangeSets.GetOrAdd(sessionId, guid => new EntityChangeSet
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

        if (!IsTypeOfEntity(typeOfEntity) /*&& !entityEntry.Metadata.IsOwned()*/)
        {
            return false;
        }

        var shouldAuditEntity = IsTypeOfAuditedEntity(typeOfEntity);
        if (shouldAuditEntity.HasValue && !shouldAuditEntity.Value)
        {
            return false;
        }

        //bool? shouldAuditOwnerEntity = null;
        //bool? shouldAuditOwnerProperty = null;
        //if (!shouldAuditEntity.HasValue && entityEntry.Metadata.IsOwned())
        //{
        //    // Check if owner entity has auditing attribute
        //    var ownerForeignKey = entityEntry.Metadata.GetForeignKeys().First(fk => fk.IsOwnership);
        //    var ownerEntityType = ownerForeignKey.PrincipalEntityType.ClrType;

        //    shouldAuditOwnerEntity = IsTypeOfAuditedEntity(ownerEntityType);
        //    if (shouldAuditOwnerEntity.HasValue && !shouldAuditOwnerEntity.Value)
        //    {
        //        return false;
        //    }

        //    var ownerPropertyInfo = ownerForeignKey.PrincipalToDependent.PropertyInfo;
        //    shouldAuditOwnerProperty = IsAuditedPropertyInfo(ownerEntityType, ownerPropertyInfo);
        //    if (shouldAuditOwnerProperty.HasValue && !shouldAuditOwnerProperty.Value)
        //    {
        //        return false;
        //    }
        //}

        return shouldAuditEntity /*?? shouldAuditOwnerEntity ?? shouldAuditOwnerProperty*/ ?? shouldTrackEntity;
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
    [CanBeNull]
    private EntityChange CreateEntityChange(PreInsertEvent @event)
    {
        var entityId = GetEntityId(@event);
        var entityTypeFullName = ProxyHelper.GetUnproxiedType(@event.Entity).FullName;
        var changeType = EntityChangeType.Created;

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
    [CanBeNull]
    private EntityChange CreateEntityChange(PreUpdateEvent @event)
    {
        var entityId = @event.Persister.GetIdentifier(@event.Entity).ToJsonString();
        var entityTypeFullName = ProxyHelper.GetUnproxiedType(@event.Entity).FullName;
        var changeType = @event.IsDeleted() ? EntityChangeType.Deleted : EntityChangeType.Updated;

        if (entityId == null)
        {
            Logger.ErrorFormat("EntityChangeType {0} must have non-empty entity id", changeType);
            return null;
        }

        return new EntityChange
        {
            ChangeType = changeType,
            EntityEntry = @event, // [NotMapped]
            EntityId = entityId,
            EntityTypeFullName = entityTypeFullName,
            TenantId = AbpSession.TenantId
        };
    }
    [CanBeNull]
    private EntityChange CreateEntityChange(PreDeleteEvent @event)
    {
        var entityId = @event.Persister.GetIdentifier(@event.Entity).ToJsonString();
        var entityTypeFullName = ProxyHelper.GetUnproxiedType(@event.Entity).FullName;
        var changeType = EntityChangeType.Deleted;

        if (entityId == null)
        {
            Logger.ErrorFormat("EntityChangeType {0} must have non-empty entity id", changeType);
            return null;
        }

        return new EntityChange
        {
            ChangeType = changeType,
            EntityEntry = @event, // [NotMapped]
            EntityId = entityId,
            EntityTypeFullName = entityTypeFullName,
            TenantId = AbpSession.TenantId
        };
    }
    private ICollection<EntityPropertyChange> GetPropertyChanges(PreInsertEvent @event, bool auditedPropertiesOnly)
    {
        var propertyChanges = new List<EntityPropertyChange>();

        var dirtyPropertyIndexes = @event.Persister.FindDirty(@event.State, new object[@event.State.Length], @event.Entity, @event.Session);

        foreach (var dirtyPropertyIndex in dirtyPropertyIndexes)
        {
            var property = @event.Persister.EntityMetamodel.Properties[dirtyPropertyIndex];

            if (property.Name == @event.Persister.IdentifierPropertyName)
            {
                continue;
            }

            var propertyIndex = @event.Persister.EntityMetamodel.GetPropertyIndex(property.Name);
            var propertyInfo = @event.Entity.GetType().GetProperty(@event.Persister.PropertyNames[propertyIndex]);
            if (ShouldSavePropertyHistory(propertyInfo, !auditedPropertiesOnly))
            {
                propertyChanges.Add(
                    CreateEntityPropertyChange(
                        null,
                        @event.State[propertyIndex],
                        property.Name,
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

        var dirtyPropertyIndexes = @event.Persister.FindDirty(@event.State, @event.OldState, @event.Entity, @event.Session);

        foreach (var dirtyPropertyIndex in dirtyPropertyIndexes)
        {
            var property = @event.Persister.EntityMetamodel.Properties[dirtyPropertyIndex];

            if (property.Name == @event.Persister.IdentifierPropertyName)
            {
                continue;
            }

            var propertyIndex = @event.Persister.EntityMetamodel.GetPropertyIndex(property.Name);
            var propertyInfo = @event.Entity.GetType().GetProperty(@event.Persister.PropertyNames[propertyIndex]);
            if (ShouldSavePropertyHistory(propertyInfo, !auditedPropertiesOnly))
            {
                propertyChanges.Add(
                    CreateEntityPropertyChange(
                        @event.OldState[propertyIndex],
                        @event.State[propertyIndex],
                        property.Name,
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

        var dirtyPropertyIndexes = @event.Persister.FindDirty(new object[@event.DeletedState.Length], @event.DeletedState, @event.Entity, @event.Session);

        foreach (var dirtyPropertyIndex in dirtyPropertyIndexes)
        {
            var property = @event.Persister.EntityMetamodel.Properties[dirtyPropertyIndex];

            if (property.Name == @event.Persister.IdentifierPropertyName)
            {
                continue;
            }

            var propertyIndex = @event.Persister.EntityMetamodel.GetPropertyIndex(property.Name);
            var propertyInfo = @event.Entity.GetType().GetProperty(@event.Persister.PropertyNames[propertyIndex]);
            if (ShouldSavePropertyHistory(propertyInfo, !auditedPropertiesOnly))
            {
                propertyChanges.Add(
                    CreateEntityPropertyChange(
                        @event.DeletedState[propertyIndex],
                        null,
                        property.Name,
                        propertyInfo
                    )
                );
            }
        }

        return propertyChanges;
    }
    private EntityPropertyChange CreateEntityPropertyChange(object oldValue, object newValue, string propertyName, PropertyInfo propertyInfo)
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