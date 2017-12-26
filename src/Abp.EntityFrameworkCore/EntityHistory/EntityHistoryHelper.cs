using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Transactions;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Extensions;
using Abp.Json;
using Abp.Runtime.Session;
using Abp.Threading;
using Abp.Timing;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Abp.EntityHistory
{
    public class EntityHistoryHelper : IEntityHistoryHelper, ITransientDependency
    {
        public ILogger Logger { get; set; }
        public IAbpSession AbpSession { get; set; }
        public IEntityHistoryStore EntityHistoryStore { get; set; }

        private readonly IEntityHistoryConfiguration _configuration;
        private readonly IUnitOfWorkManager _unitOfWorkManager;

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
            IUnitOfWorkManager unitOfWorkManager)
        {
            _configuration = configuration;
            _unitOfWorkManager = unitOfWorkManager;

            AbpSession = NullAbpSession.Instance;
            Logger = NullLogger.Instance;
            EntityHistoryStore = NullEntityHistoryStore.Instance;
        }
        
        public EntityChangeSet CreateEntityChangeSet(ICollection<EntityEntry> entityEntries)
        {
            if (!IsEntityHistoryEnabled)
            {
                return null;
            }

            var changeSet = new EntityChangeSet();

            foreach (var entry in entityEntries)
            {
                if (ShouldSaveEntityHistory(entry))
                {
                    var entityChangeInfo = CreateEntityChangeInfo(entry);
                    if (entityChangeInfo == null)
                    {
                        continue;
                    }
                    changeSet.EntityChanges.Add(entityChangeInfo);
                }
            }

            return changeSet;
        }

        private bool ShouldSaveEntityHistory(EntityEntry entityEntry, bool defaultValue = false)
        {
            if (entityEntry == null)
            {
                return false;
            }

            var entityState = entityEntry.State;
            if (entityState == EntityState.Detached)
            {
                return false;
            }

            if (entityState == EntityState.Unchanged)
            {
                return false;
            }

            var entity = entityEntry.Entity;
            if (entity == null)
            {
                return false;
            }

            if (_configuration.IgnoredTypes.Any(t => t.IsInstanceOfType(entity)))
            {
                return false;
            }

            var entityType = entity.GetType();
            if (!EntityHelper.IsEntity(entityType))
            {
                return false;
            }

            if (!entityType.IsPublic)
            {
                return false;
            }

            if (entityType.GetTypeInfo().IsDefined(typeof(HistoryTrackedAttribute), true))
            {
                return true;
            }

            if (entityType.GetTypeInfo().IsDefined(typeof(DisableHistoryTrackingAttribute), true))
            {
                return false;
            }

            if (_configuration.Selectors.Any(selector => selector.Predicate(entityType)))
            {
                return true;
            }

            return defaultValue;
        }

        private bool ShouldSavePropertyHistory(PropertyEntry propertyEntry, bool defaultValue)
        {
            var propertyInfo = propertyEntry.Metadata.PropertyInfo;
            if (propertyInfo.IsDefined(typeof(DisableHistoryTrackingAttribute), true))
            {
                return false;
            }

            var classType = propertyInfo.DeclaringType;
            if (classType != null)
            {
                if (classType.GetTypeInfo().IsDefined(typeof(DisableHistoryTrackingAttribute), true) &&
                    !propertyInfo.IsDefined(typeof(HistoryTrackedAttribute), true))
                {
                    return false;
                }
            }

            var isModified = propertyEntry.OriginalValue.ToJsonString() != propertyEntry.CurrentValue.ToJsonString();
            if (isModified)
            {
                return true;
            }

            return defaultValue;
        }

        public void Save(EntityChangeSet changeSet)
        {
            AsyncHelper.RunSync(() => SaveAsync(changeSet));
        }

        public async Task SaveAsync(EntityChangeSet changeSet)
        {
            if (changeSet == null || !IsEntityHistoryEnabled)
            {
                return;
            }

            if (changeSet.EntityChanges.Count == 0)
            {
                return;
            }

            UpdateChangeSet(changeSet);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await EntityHistoryStore.SaveAsync(changeSet);
                await uow.CompleteAsync();
            }
        }

        private EntityChangeInfo CreateEntityChangeInfo(EntityEntry entityEntry)
        {
            var entity = entityEntry.Entity;
            
            DateTime changeTime;
            EntityChangeType changeType;
            switch (entityEntry.State)
            {
                case EntityState.Added:
                    changeType = EntityChangeType.Created;
                    changeTime = GetCreationTime(entity);
                    break;
                case EntityState.Deleted:
                    changeType = EntityChangeType.Deleted;
                    changeTime = GetDeletionTime(entity);
                    break;
                case EntityState.Modified:
                    if (IsDeleted(entityEntry))
                    {
                        changeType = EntityChangeType.Deleted;
                        changeTime = GetDeletionTime(entity);
                    }
                    else
                    {
                        changeType = EntityChangeType.Updated;
                        changeTime = GetLastModificationTime(entity);
                    }
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                default:
                    return null;
            }

            var entityId = GetEntityId(entity);
            if (entityId == null && changeType != EntityChangeType.Created)
            {
                return null;
            }

            var entityType = entity.GetType();
            var entityChangeInfo = new EntityChangeInfo
            {
                TenantId = AbpSession.TenantId,
                UserId = AbpSession.UserId,
                ImpersonatorUserId = AbpSession.ImpersonatorUserId,
                ImpersonatorTenantId = AbpSession.ImpersonatorTenantId,
                ChangeTime = changeTime,
                ChangeType = changeType,
                EntityEntry = entityEntry, // [NotMapped]
                EntityId = entityId,
                EntityTypeAssemblyQualifiedName = entityType.AssemblyQualifiedName,
                PropertyChanges = GetPropertyChanges(entityEntry)
            };

            return entityChangeInfo;
        }

        private DateTime GetCreationTime(object entityAsObj)
        {
            return (entityAsObj as IHasCreationTime)?.CreationTime ?? Clock.Now;
        }

        private DateTime GetDeletionTime(object entityAsObj)
        {
            return (entityAsObj as IHasDeletionTime)?.DeletionTime ?? Clock.Now;
        }

        private DateTime GetLastModificationTime(object entityAsObj)
        {
            return (entityAsObj as IHasModificationTime)?.LastModificationTime ?? Clock.Now;
        }

        private string GetEntityId(object entityAsObj)
        {
            return entityAsObj
                .GetType().GetProperty("Id")?
                .GetValue(entityAsObj)?
                .ToJsonString();
        }

        /// <summary>
        /// Gets the property changes for this entry.
        /// </summary>
        private ICollection<EntityPropertyChangeInfo> GetPropertyChanges(EntityEntry entityEntry)
        {
            var propertyChanges = new List<EntityPropertyChangeInfo>();
            var properties = entityEntry.Metadata.GetProperties();
            var isCreated = entityEntry.State == EntityState.Added;
            var isCreatedOrDeleted = isCreated || IsDeleted(entityEntry);

            foreach (var property in properties)
            {
                var propertyEntry = entityEntry.Property(property.Name);
                if (ShouldSavePropertyHistory(propertyEntry, isCreatedOrDeleted))
                {
                    propertyChanges.Add(new EntityPropertyChangeInfo
                    {
                        NewValue = propertyEntry.CurrentValue.ToJsonString(),
                        OriginalValue = propertyEntry.OriginalValue.ToJsonString(),
                        PropertyName = property.Name,
                        PropertyTypeName = property.ClrType.AssemblyQualifiedName
                    });
                }
            }

            #region Get foreign keys

            var foreignKeys = entityEntry.Metadata.GetForeignKeys();

            foreach (var foreignKey in foreignKeys)
            {
                foreach (var property in foreignKey.Properties)
                {
                    var propertyEntry = entityEntry.Property(property.Name);
                    propertyChanges.Add(new EntityPropertyChangeInfo
                    {
                        NewValue = propertyEntry.CurrentValue.ToJsonString(),
                        OriginalValue = propertyEntry.OriginalValue.ToJsonString(),
                        PropertyName = property.Name,
                        PropertyTypeName = property.ClrType.AssemblyQualifiedName
                    });
                }
            }

            #endregion

            return propertyChanges;
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

        /// <summary>
        /// Updates entity id and foreign keys after SaveChanges is called.
        /// </summary>
        private void UpdateChangeSet(EntityChangeSet changeSet)
        {
            foreach (var entityChangeInfo in changeSet.EntityChanges)
            {
                var entityEntry = entityChangeInfo.EntityEntry.As<EntityEntry>();
                entityChangeInfo.EntityId = GetEntityId(entityEntry.Entity);

                #region Update foreign keys

                var foreignKeys = entityEntry.Metadata.GetForeignKeys();

                foreach (var foreignKey in foreignKeys)
                {
                    foreach (var property in foreignKey.Properties)
                    {
                        var propertyEntry = entityEntry.Property(property.Name);
                        var propertyChange = entityChangeInfo.PropertyChanges
                            .Where(pc => pc.PropertyName == property.Name)
                            .FirstOrDefault();

                        if (propertyChange == null)
                        {
                            continue;
                        }

                        if (propertyChange.OriginalValue == propertyChange.NewValue)
                        {
                            var newValue = propertyEntry.CurrentValue.ToJsonString();
                            if (newValue == propertyChange.NewValue)
                            {
                                // No change
                                entityChangeInfo.PropertyChanges.Remove(propertyChange);
                            }
                            else
                            {
                                propertyChange.NewValue = newValue;
                            }
                        }
                    }
                }

                #endregion
            }
        }
    }
}
