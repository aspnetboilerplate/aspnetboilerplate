using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
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
using Abp.Timing;
using Castle.Core.Logging;
using JetBrains.Annotations;

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
            EntityChangeSetReasonProvider = NullEntityChangeSetReasonProvider.Instance;
            EntityHistoryStore = NullEntityHistoryStore.Instance;
        }

        public virtual EntityChangeSet CreateEntityChangeSet(DbContext context)
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

            foreach (var entry in context.ChangeTracker.Entries())
            {
                var shouldSaveEntityHistory = ShouldSaveEntityHistory(entry);
                if (!shouldSaveEntityHistory && !HasAuditedProperties(entry))
                {
                    continue;
                }

                var entityChange = CreateEntityChange(entry, GetEntityType(context, entry), shouldSaveEntityHistory);
                if (entityChange == null)
                {
                    continue;
                }

                changeSet.EntityChanges.Add(entityChange);
            }

            return changeSet;
        }

        public virtual async Task SaveAsync(DbContext context, EntityChangeSet changeSet)
        {
            if (!IsEntityHistoryEnabled)
            {
                return;
            }

            if (changeSet.EntityChanges.Count == 0)
            {
                return;
            }

            UpdateChangeSet(context, changeSet);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                await EntityHistoryStore.SaveAsync(changeSet);
                await uow.CompleteAsync();
            }
        }

        [CanBeNull]
        private EntityChange CreateEntityChange(DbEntityEntry entityEntry, EntityType entityType, bool shouldSaveEntityHistory)
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

            var entityId = GetEntityId(entityEntry, entityType);
            if (entityId == null && changeType != EntityChangeType.Created)
            {
                Logger.Error("Unexpected null value for entityId!");
                return null;
            }

            var entityChange = new EntityChange
            {
                ChangeType = changeType,
                EntityEntry = entityEntry, // [NotMapped]
                EntityId = entityId,
                EntityTypeFullName = entityType.FullName,
                PropertyChanges = GetPropertyChanges(entityEntry, entityType, shouldSaveEntityHistory),
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
            var entity = entityChange.EntityEntry.As<DbEntityEntry>().Entity;
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

        private Type GetEntityBaseType(DbEntityEntry entityEntry)
        {
            return ObjectContext.GetObjectType(entityEntry.Entity.GetType());
        }

        private EntityType GetEntityType(ObjectContext context, Type entityClrType)
        {
            var metadataWorkspace = context.MetadataWorkspace;
            /* Get the mapping between Clr types in OSpace */
            var objectItemCollection = ((ObjectItemCollection)metadataWorkspace.GetItemCollection(DataSpace.OSpace));
            return metadataWorkspace
                .GetItems<EntityType>(DataSpace.OSpace)
                .Single(e => objectItemCollection.GetClrType(e) == entityClrType);
        }

        private EntitySet GetEntitySet(ObjectContext context, EntityType entityType)
        {
            var metadataWorkspace = context.MetadataWorkspace;
            /* Get the mapping between entity set/type in CSpace */
            return metadataWorkspace
                .GetItems<EntityContainer>(DataSpace.CSpace)
                .Single()
                .EntitySets
                .Single(e => e.ElementType.Name == entityType.Name);
        }

        private string GetEntityId(DbEntityEntry entityEntry, EntityType entityType)
        {
            var primaryKey = entityType.KeyProperties.First();
            return entityEntry.Property(primaryKey.Name)?.CurrentValue?.ToJsonString();
        }

        /// <summary>
        /// Gets the property changes for this entry.
        /// </summary>
        private ICollection<EntityPropertyChange> GetPropertyChanges(DbEntityEntry entityEntry, EntityType entityType, bool shouldSaveEntityHistory)
        {
            var propertyChanges = new List<EntityPropertyChange>();
            var propertyNames = entityEntry.CurrentValues.PropertyNames;
            var isCreated = IsCreated(entityEntry);
            var isDeleted = IsDeleted(entityEntry);

            foreach (var propertyName in propertyNames)
            {
                var propertyEntry = entityEntry.Property(propertyName);
                if (propertyEntry is DbComplexPropertyEntry)
                {
                    continue;
                }
                if (ShouldSavePropertyHistory(propertyEntry, entityType, shouldSaveEntityHistory, isCreated || isDeleted))
                {
                    var propertyInfo = entityEntry.Entity.GetType().GetProperty(propertyEntry.Name);
                    propertyChanges.Add(new EntityPropertyChange
                    {
                        NewValue = isDeleted ? null : propertyEntry.CurrentValue.ToJsonString().TruncateWithPostfix(EntityPropertyChange.MaxValueLength),
                        OriginalValue = isCreated ? null : propertyEntry.OriginalValue.ToJsonString().TruncateWithPostfix(EntityPropertyChange.MaxValueLength),
                        PropertyName = propertyEntry.Name,
                        PropertyTypeFullName = propertyInfo.PropertyType.FullName,
                        TenantId = AbpSession.TenantId
                    });
                }
            }

            return propertyChanges;
        }

        private bool HasAuditedProperties(DbEntityEntry entityEntry)
        {
            var propertyNames = entityEntry.CurrentValues.PropertyNames;
            var entityType = entityEntry.Entity.GetType();
            return propertyNames.Any(p => entityType.GetProperty(p)?.IsDefined(typeof(AuditedAttribute)) ?? false);
        }

        private bool IsCreated(DbEntityEntry entityEntry)
        {
            return entityEntry.State == EntityState.Added;
        }

        private bool IsDeleted(DbEntityEntry entityEntry)
        {
            if (entityEntry.State == EntityState.Deleted)
            {
                return true;
            }

            var entity = entityEntry.Entity;
            return entity is ISoftDelete && entity.As<ISoftDelete>().IsDeleted;
        }

        private bool ShouldSaveEntityHistory(DbEntityEntry entityEntry)
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

            var shouldSaveEntityHistoryForType = ShouldSaveEntityHistoryForType(entityType);
            if (shouldSaveEntityHistoryForType.HasValue)
            {
                return shouldSaveEntityHistoryForType.Value;
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

        private bool ShouldSavePropertyHistory(DbPropertyEntry propertyEntry, EntityType entityType, bool shouldSaveEntityHistory, bool defaultValue)
        {
            if (entityType.KeyMembers.Any(k => k.Name ==  propertyEntry.Name))
            {
                return false;
            }

            var propertyInfo = propertyEntry.EntityEntry.Entity.GetType().GetProperty(propertyEntry.Name);

            var shouldSavePropertyHistoryForInfo = ShouldSavePropertyHistoryForInfo(propertyInfo, shouldSaveEntityHistory);
            if (shouldSavePropertyHistoryForInfo.HasValue)
            {
                return shouldSavePropertyHistoryForInfo.Value;
            }

            var isModified = false;
            if (propertyEntry.EntityEntry.State == EntityState.Added)
            {
                isModified = propertyEntry.CurrentValue != null;
            }
            else
            {
                isModified = !(propertyEntry.OriginalValue?.Equals(propertyEntry.CurrentValue) ?? propertyEntry.CurrentValue == null);
            }
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
        private void UpdateChangeSet(DbContext context, EntityChangeSet changeSet)
        {
            foreach (var entityChange in changeSet.EntityChanges)
            {
                /* Update change time */

                entityChange.ChangeTime = GetChangeTime(entityChange);

                /* Update entity id */

                var entityEntry = entityChange.EntityEntry.As<DbEntityEntry>();
                entityChange.EntityId = GetEntityId(entityEntry, GetEntityType(context, entityEntry));

                /* Update foreign keys */

                var entityType = GetEntityType(context, entityEntry);
                var foreignKeys = entityType.NavigationProperties;

                foreach (var foreignKey in foreignKeys)
                {
                    foreach (var property in foreignKey.GetDependentProperties())
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
                                    PropertyTypeFullName = property.TypeUsage.EdmType.FullName
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
    }
}
