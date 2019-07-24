using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Auditing;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFramework;
using Abp.EntityFrameworkCore;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Extensions;
using Abp.Json;
using Abp.Runtime.Session;
using Abp.Timing;
using Castle.Core.Logging;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Abp.EntityHistory
{
    public class EntityHistoryHelper : IEntityHistoryHelper, ITransientDependency
    {
        public ILogger Logger { get; set; }
        public IAbpSession AbpSession { get; set; }
        public IClientInfoProvider ClientInfoProvider { get; set; }
        public IEntityChangeSetReasonProvider EntityChangeSetReasonProvider { get; set; }
        public IEntityHistoryStore EntityHistoryStore { get; set; }

        private readonly IEntityHistoryConfiguration _configuration;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<EntityChange, long> _entityChangeRepository;

        private bool IsEntityHistoryEnabled
        {
            get
            {
                if (!_configuration.IsEnabled)
                {
                    return false;
                }

                if (!_configuration.IsEnabledForAnonymousUsers && (AbpSession?.UserId == null))
                {
                    return false;
                }

                return true;
            }
        }

        public EntityHistoryHelper(
            IEntityHistoryConfiguration configuration,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<EntityChange, long> entityChangeRepository)
        {
            _configuration = configuration;
            _unitOfWorkManager = unitOfWorkManager;
            _entityChangeRepository = entityChangeRepository;

            AbpSession = NullAbpSession.Instance;
            Logger = NullLogger.Instance;
            ClientInfoProvider = NullClientInfoProvider.Instance;
            EntityChangeSetReasonProvider = NullEntityChangeSetReasonProvider.Instance;
            EntityHistoryStore = NullEntityHistoryStore.Instance;
        }

        public virtual EntityChangeSet CreateEntityChangeSet(ICollection<EntityEntry> entityEntries)
        {
            var changeSet = new EntityChangeSet
            {
                Reason = EntityChangeSetReasonProvider.Reason.TruncateWithPostfix(EntityChangeSet.MaxReasonLength),

                // Fill "who did this change"
                BrowserInfo = ClientInfoProvider.BrowserInfo.TruncateWithPostfix(EntityChangeSet.MaxBrowserInfoLength),
                ClientIpAddress = ClientInfoProvider.ClientIpAddress.TruncateWithPostfix(EntityChangeSet.MaxClientIpAddressLength),
                ClientName = ClientInfoProvider.ComputerName.TruncateWithPostfix(EntityChangeSet.MaxClientNameLength),
                ImpersonatorTenantId = AbpSession.ImpersonatorTenantId,
                ImpersonatorUserId = AbpSession.ImpersonatorUserId,
                TenantId = AbpSession.TenantId,
                UserId = AbpSession.UserId
            };

            if (!IsEntityHistoryEnabled)
            {
                return changeSet;
            }

            foreach (var entry in entityEntries)
            {
                var shouldSaveEntityHistory = ShouldSaveEntityHistory(entry);
                if (!shouldSaveEntityHistory && !HasAuditedProperties(entry))
                {
                    continue;
                }

                var entityChange = CreateEntityChange(entry, shouldSaveEntityHistory);
                if (entityChange == null)
                {
                    continue;
                }

                changeSet.EntityChanges.Add(entityChange);
            }

            return changeSet;
        }

        public virtual async Task SaveAsync(EntityChangeSet changeSet)
        {
            if (!IsEntityHistoryEnabled)
            {
                return;
            }

            if (changeSet.EntityChanges.Count == 0)
            {
                return;
            }

            UpdateChangeSet(changeSet);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                await EntityHistoryStore.SaveAsync(changeSet);
                await uow.CompleteAsync();
            }
        }

        protected virtual string GetEntityId(EntityEntry entry)
        {
            var primaryKeys = entry.Properties.Where(p => p.Metadata.IsPrimaryKey());
            return primaryKeys.First().CurrentValue?.ToJsonString();
        }

        [CanBeNull]
        private EntityChange CreateEntityChange(EntityEntry entityEntry, bool shouldSaveEntityHistory)
        {
            EntityChangeType changeType;
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    changeType = EntityChangeType.Created;
                    break;
                case EntityState.Deleted:
                    changeType = EntityChangeType.Deleted;
                    break;
                case EntityState.Modified:
                    changeType = IsDeleted(entityEntry) ? EntityChangeType.Deleted : EntityChangeType.Updated;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    Logger.Error("Unexpected EntityState!");
                    return null;
            }

            var entityId = GetEntityId(entityEntry);
            if (entityId == null && changeType != EntityChangeType.Created)
            {
                Logger.Error("Unexpected null value for entityId!");
                return null;
            }

            var entityType = entityEntry.Entity.GetType();
            var entityChange = new EntityChange
            {
                ChangeType = changeType,
                EntityEntry = entityEntry, // [NotMapped]
                EntityId = entityId,
                EntityTypeFullName = entityType.FullName,
                PropertyChanges = GetPropertyChanges(entityEntry, shouldSaveEntityHistory),
                TenantId = AbpSession.TenantId
            };

            if (!shouldSaveEntityHistory && entityChange.PropertyChanges.Count == 0)
            {
                return null;
            }

            return entityChange;
        }

        private DateTime GetChangeTime(EntityChange entityChange)
        {
            var entity = entityChange.EntityEntry.As<EntityEntry>().Entity;
            switch (entityChange.ChangeType)
            {
                case EntityChangeType.Created:
                    return (entity as IHasCreationTime)?.CreationTime ?? Clock.Now;
                case EntityChangeType.Deleted:
                    return (entity as IHasDeletionTime)?.DeletionTime ?? Clock.Now;
                case EntityChangeType.Updated:
                    return (entity as IHasModificationTime)?.LastModificationTime ?? Clock.Now;
                default:
                    Logger.Error("Unexpected EntityState!");
                    return Clock.Now;
            }
        }

        /// <summary>
        /// Gets the property changes for this entry.
        /// </summary>
        private ICollection<EntityPropertyChange> GetPropertyChanges(EntityEntry entityEntry, bool shouldSaveEntityHistory)
        {
            var propertyChanges = new List<EntityPropertyChange>();
            var properties = entityEntry.Metadata.GetProperties();
            var isCreated = IsCreated(entityEntry);
            var isDeleted = IsDeleted(entityEntry);

            foreach (var property in properties)
            {
                var propertyEntry = entityEntry.Property(property.Name);
                if (ShouldSavePropertyHistory(propertyEntry, shouldSaveEntityHistory, isCreated || isDeleted))
                {
                    propertyChanges.Add(new EntityPropertyChange
                    {
                        NewValue = isDeleted ? null : propertyEntry.CurrentValue.ToJsonString().TruncateWithPostfix(EntityPropertyChange.MaxValueLength),
                        OriginalValue = isCreated ? null : propertyEntry.OriginalValue.ToJsonString().TruncateWithPostfix(EntityPropertyChange.MaxValueLength),
                        PropertyName = property.Name,
                        PropertyTypeFullName = property.ClrType.FullName,
                        TenantId = AbpSession.TenantId
                    });
                }
            }

            return propertyChanges;
        }

        private bool HasAuditedProperties(EntityEntry entityEntry)
        {
            var properties = entityEntry.Metadata.GetProperties();
            return properties.Any(p => p.PropertyInfo?.IsDefined(typeof(AuditedAttribute)) ?? false);
        }

        private bool IsCreated(EntityEntry entityEntry)
        {
            return entityEntry.State == EntityState.Added;
        }

        private bool IsDeleted(EntityEntry entityEntry)
        {
            if (entityEntry.State == EntityState.Deleted)
            {
                return true;
            }

            var entity = entityEntry.Entity;
            return entity is ISoftDelete && entity.As<ISoftDelete>().IsDeleted;
        }

        private bool ShouldSaveEntityHistory(EntityEntry entityEntry)
        {
            if (entityEntry.State == EntityState.Detached ||
                entityEntry.State == EntityState.Unchanged)
            {
                return false;
            }

            if (_configuration.IgnoredTypes.Any(t => t.IsInstanceOfType(entityEntry.Entity)))
            {
                return false;
            }

            var entityType = entityEntry.Entity.GetType();
            var isOwnedEntity = entityEntry.Metadata.IsOwned();

            if (!EntityHelper.IsEntity(entityType) && !isOwnedEntity)
            {
                return false;
            }

            var shouldSaveEntityHistoryForType = ShouldSaveEntityHistoryForType(entityType);
            if (shouldSaveEntityHistoryForType.HasValue)
            {
                return shouldSaveEntityHistoryForType.Value;
            }

            if (isOwnedEntity)
            {
                // Check if should save entity history for property that points to this owned entity
                var foreignKey = entityEntry.Metadata.GetForeignKeys().First();
                var propertyInfo = foreignKey.PrincipalToDependent.PropertyInfo;
                var ownerType = foreignKey.PrincipalEntityType.ClrType;
                var shouldSaveEntityHistoryForOwnerType = ShouldSaveEntityHistoryForType(ownerType);
                var shouldSaveEntityHistoryForProperty = ShouldSavePropertyHistoryForInfo(propertyInfo, shouldSaveEntityHistoryForOwnerType ?? false);
                if (shouldSaveEntityHistoryForProperty.HasValue)
                {
                    return shouldSaveEntityHistoryForProperty.Value;
                }

                // Check if should save entity history for owner type
                if (shouldSaveEntityHistoryForOwnerType.HasValue)
                {
                    return shouldSaveEntityHistoryForOwnerType.Value;
                }
            }

            return false;
        }

        private bool? ShouldSaveEntityHistoryForType(Type entityType)
        {
            if (!entityType.IsPublic)
            {
                return false;
            }

            if (entityType.GetTypeInfo().IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            if (entityType.GetTypeInfo().IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            if (_configuration.Selectors.Any(selector => selector.Predicate(entityType)))
            {
                return true;
            }

            return null;
        }

        private bool ShouldSavePropertyHistory(PropertyEntry propertyEntry, bool shouldSaveEntityHistory, bool defaultValue)
        {
            if (propertyEntry.Metadata.IsPrimaryKey())
            {
                return false;
            }

            var propertyInfo = propertyEntry.Metadata.PropertyInfo;

            var shouldSavePropertyHistoryForInfo = ShouldSavePropertyHistoryForInfo(propertyInfo, shouldSaveEntityHistory);
            if (shouldSavePropertyHistoryForInfo.HasValue)
            {
                return shouldSavePropertyHistoryForInfo.Value;
            }

            var isModified = !(propertyEntry.OriginalValue?.Equals(propertyEntry.CurrentValue) ?? propertyEntry.CurrentValue == null);
            if (isModified)
            {
                return true;
            }

            return defaultValue;
        }

        private bool? ShouldSavePropertyHistoryForInfo(PropertyInfo propertyInfo, bool shouldSaveEntityHistory)
        {
            if (propertyInfo != null && propertyInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            if (!shouldSaveEntityHistory)
            {
                // Should not save property history if property is not audited
                if (propertyInfo == null || !propertyInfo.IsDefined(typeof(AuditedAttribute), true))
                {
                    return false;
                }
            }

            return null;
        }

        /// <summary>
        /// Updates change time, entity id and foreign keys after SaveChanges is called.
        /// </summary>
        private void UpdateChangeSet(EntityChangeSet changeSet)
        {
            foreach (var entityChange in changeSet.EntityChanges)
            {
                /* Update change time */

                entityChange.ChangeTime = GetChangeTime(entityChange);

                /* Update entity id */

                var entityEntry = entityChange.EntityEntry.As<EntityEntry>();
                entityChange.EntityId = GetEntityId(entityEntry);

                /* Update foreign keys */

                var foreignKeys = entityEntry.Metadata.GetForeignKeys();

                foreach (var foreignKey in foreignKeys)
                {
                    foreach (var property in foreignKey.Properties)
                    {
                        var propertyEntry = entityEntry.Property(property.Name);
                        var propertyChange = entityChange.PropertyChanges.FirstOrDefault(pc => pc.PropertyName == property.Name);

                        if (propertyChange == null)
                        {
                            if (!(propertyEntry.OriginalValue?.Equals(propertyEntry.CurrentValue) ?? propertyEntry.CurrentValue == null))
                            {
                                // Add foreign key
                                entityChange.PropertyChanges.Add(new EntityPropertyChange
                                {
                                    NewValue = propertyEntry.CurrentValue.ToJsonString(),
                                    OriginalValue = propertyEntry.OriginalValue.ToJsonString(),
                                    PropertyName = property.Name,
                                    PropertyTypeFullName = property.ClrType.FullName
                                });
                            }

                            continue;
                        }

                        if (propertyChange.OriginalValue == propertyChange.NewValue)
                        {
                            var newValue = propertyEntry.CurrentValue.ToJsonString();
                            if (newValue == propertyChange.NewValue)
                            {
                                // No change
                                entityChange.PropertyChanges.Remove(propertyChange);
                            }
                            else
                            {
                                // Update foreign key
                                propertyChange.NewValue = newValue.TruncateWithPostfix(EntityPropertyChange.MaxValueLength);
                            }
                        }
                    }
                }
            }
        }
        public async Task<EntityHistorySnapshot> GetEntitySnapshotAsync<TEntity>(int id, DateTime snapshotTime) where TEntity : class, IEntity<int>
        {
            return await GetEntitySnapshotAsync<TEntity, int>(id, snapshotTime);
        }
        public async Task<EntityHistorySnapshot> GetEntitySnapshotAsync<TEntity, TPrimaryKey>(TPrimaryKey id, DateTime snapshotTime) where TEntity : class, IEntity<TPrimaryKey>
        {
            var entity = _entityChangeRepository.GetDbContext()
                .Set<TEntity>().AsQueryable().FirstOrDefault(x => x.Id.Equals(id));

            var snapshotPropertiesDictionary = new Dictionary<string, string>();
            var propertyChangesStackTreeDictionary = new Dictionary<string, string>();

            if (entity != null)
            {
                string fullName = typeof(TEntity).FullName;
                var idJson = id.ToJsonString();

                var allChanges = _entityChangeRepository.GetAll().Include(x => x.PropertyChanges).ToList();
                var changes = await _entityChangeRepository.GetAll()//select all changes which created after snapshot time 
                    .Where(x => x.EntityTypeFullName == fullName && x.EntityId == idJson && x.ChangeTime > snapshotTime && x.ChangeType != EntityChangeType.Created)
                    .OrderByDescending(x => x.ChangeTime)
                    .Select(x => new { x.ChangeType, x.PropertyChanges }).ToListAsync();

                //and revoke all changes

                foreach (var changedProperties in changes)// desc ordered changes
                {
                    foreach (var entityPropertyChange in changedProperties.PropertyChanges)
                    {
                        if (snapshotPropertiesDictionary.ContainsKey(entityPropertyChange.PropertyName))
                        {
                            snapshotPropertiesDictionary[entityPropertyChange.PropertyName] = entityPropertyChange.OriginalValue;//set back to orginal value
                        }
                        else
                        {
                            snapshotPropertiesDictionary.Add(entityPropertyChange.PropertyName, entityPropertyChange.OriginalValue);//set back to orginal value
                        }

                        //create change stack tree
                        if (propertyChangesStackTreeDictionary.ContainsKey(entityPropertyChange.PropertyName))
                        {
                            propertyChangesStackTreeDictionary[entityPropertyChange.PropertyName] += " -> " + entityPropertyChange.OriginalValue;
                        }
                        else
                        {
                            string propertyCurrentValue = "PropertyNotExist";

                            var propertyInfo = typeof(TEntity).GetProperty(entityPropertyChange.PropertyName);
                            if (propertyInfo != null)
                            {
                                var val = propertyInfo.GetValue(entity);
                                if (val == null)
                                {
                                    propertyCurrentValue = "null";
                                }
                                else
                                {
                                    propertyCurrentValue = val.ToJsonString();
                                }
                            }
                            propertyChangesStackTreeDictionary.Add(entityPropertyChange.PropertyName, propertyCurrentValue + " -> " + entityPropertyChange.OriginalValue);
                        }
                    }
                }
            }
            return new EntityHistorySnapshot(snapshotPropertiesDictionary, propertyChangesStackTreeDictionary);
        }


    }
}
