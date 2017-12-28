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
        public IClientInfoProvider ClientInfoProvider { get; set; }
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
            ClientInfoProvider = NullClientInfoProvider.Instance;
            EntityHistoryStore = NullEntityHistoryStore.Instance;
        }
        
        public EntityChangeSet CreateEntityChangeSet(ICollection<EntityEntry> entityEntries)
        {
            var changeSet = new EntityChangeSet();

            if (!IsEntityHistoryEnabled)
            {
                return changeSet;
            }

            foreach (var entry in entityEntries)
            {
                if (ShouldSaveEntityHistory(entry))
                {
                    var entityChangeInfo = CreateEntityChangeInfo(entry);
                    if (entityChangeInfo == null)
                    {
                        // Already logged in CreateEntityChangeInfo
                        continue;
                    }
                    changeSet.EntityChanges.Add(entityChangeInfo);
                }
            }

            return changeSet;
        }

        public void Save(EntityChangeSet changeSet)
        {
            AsyncHelper.RunSync(() => SaveAsync(changeSet));
        }

        public async Task SaveAsync(EntityChangeSet changeSet)
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
                    Logger.Error("Unexpected EntityState!");
                    return null;
            }

            var entityId = GetEntityId(entity);
            if (entityId == null && changeType != EntityChangeType.Created)
            {
                Logger.Error("Unexpected null value for entityId!");
                return null;
            }

            var entityType = entity.GetType();
            var entityChangeInfo = new EntityChangeInfo
            {
                // Fill "who did this change"
                BrowserInfo = ClientInfoProvider.BrowserInfo.TruncateWithPostfix(EntityChangeInfo.MaxBrowserInfoLength),
                ClientIpAddress = ClientInfoProvider.ClientIpAddress.TruncateWithPostfix(EntityChangeInfo.MaxClientIpAddressLength),
                ClientName = ClientInfoProvider.ComputerName.TruncateWithPostfix(EntityChangeInfo.MaxClientNameLength),
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
            var isCreatedOrDeleted = IsCreated(entityEntry) || IsDeleted(entityEntry);

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

            return propertyChanges;
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

        private bool ShouldSaveEntityHistory(EntityEntry entityEntry, bool defaultValue = false)
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

            var isModified = !(propertyEntry.OriginalValue?.Equals(propertyEntry.CurrentValue) ?? propertyEntry.CurrentValue == null);
            if (isModified)
            {
                return true;
            }

            return defaultValue;
        }

        /// <summary>
        /// Updates entity id and foreign keys after SaveChanges is called.
        /// </summary>
        private void UpdateChangeSet(EntityChangeSet changeSet)
        {
            foreach (var entityChangeInfo in changeSet.EntityChanges)
            {
                /* Update entity id */

                var entityEntry = entityChangeInfo.EntityEntry.As<EntityEntry>();
                entityChangeInfo.EntityId = GetEntityId(entityEntry.Entity);

                /* Update foreign keys */

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
                            if (!(propertyEntry.OriginalValue?.Equals(propertyEntry.CurrentValue) ?? propertyEntry.CurrentValue == null))
                            {
                                // Add foreign key
                                entityChangeInfo.PropertyChanges.Add(new EntityPropertyChangeInfo
                                {
                                    NewValue = propertyEntry.CurrentValue.ToJsonString(),
                                    OriginalValue = propertyEntry.OriginalValue.ToJsonString(),
                                    PropertyName = property.Name,
                                    PropertyTypeName = property.ClrType.AssemblyQualifiedName
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
                                entityChangeInfo.PropertyChanges.Remove(propertyChange);
                            }
                            else
                            {
                                // Update foreign key
                                propertyChange.NewValue = newValue;
                            }
                        }
                    }
                }
            }
        }
    }
}
