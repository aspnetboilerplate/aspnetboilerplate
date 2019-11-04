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
using Abp.Dependency;
using Abp.Domain.Uow;
using Abp.EntityHistory.Extensions;
using Abp.Events.Bus.Entities;
using Abp.Extensions;
using Abp.Json;
using JetBrains.Annotations;

namespace Abp.EntityHistory
{
    public class EntityHistoryHelper : EntityHistoryHelperBase, IEntityHistoryHelper, ITransientDependency
    {
        public EntityHistoryHelper(
            IEntityHistoryConfiguration configuration,
            IUnitOfWorkManager unitOfWorkManager)
            : base(configuration, unitOfWorkManager)
        {
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

            var objectContext = context.As<IObjectContextAdapter>().ObjectContext;
            var relationshipChanges = objectContext.ObjectStateManager
                .GetObjectStateEntries(EntityState.Added | EntityState.Deleted)
                .Where(state => state.IsRelationship)
                .ToList();

            foreach (var entityEntry in context.ChangeTracker.Entries())
            {
                var typeOfEntity = entityEntry.GetEntityBaseType();
                var shouldTrackEntity = IsTypeOfTrackedEntity(typeOfEntity);
                if (shouldTrackEntity.HasValue && !shouldTrackEntity.Value)
                {
                    continue;
                }

                if (!IsTypeOfEntity(typeOfEntity))
                {
                    continue;
                }

                var shouldAuditEntity = IsTypeOfAuditedEntity(typeOfEntity);
                if (shouldAuditEntity.HasValue && !shouldAuditEntity.Value)
                {
                    continue;
                }

                var entityType = GetEntityType(objectContext, typeOfEntity);
                var entityChange = CreateEntityChange(entityEntry, entityType);
                if (entityChange == null)
                {
                    continue;
                }

                var shouldSaveAuditedPropertiesOnly = !shouldAuditEntity.HasValue && !entityEntry.IsCreated() && !entityEntry.IsDeleted();
                var entitySet = GetEntitySet(objectContext, entityType);

                var propertyChanges = new List<EntityPropertyChange>();
                propertyChanges.AddRange(GetPropertyChanges(entityEntry, entityType, entitySet, shouldSaveAuditedPropertiesOnly));
                propertyChanges.AddRange(GetRelationshipChanges(entityEntry, entityType, entitySet, relationshipChanges, shouldSaveAuditedPropertiesOnly));
                if (propertyChanges.Count == 0)
                {
                    continue;
                }

                entityChange.PropertyChanges = propertyChanges;
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

            UpdateChangeSet(context, changeSet);

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

        public virtual void Save(DbContext context, EntityChangeSet changeSet)
        {
            if (!IsEntityHistoryEnabled)
            {
                return;
            }

            UpdateChangeSet(context, changeSet);

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

        [CanBeNull]
        protected virtual string GetEntityId(DbEntityEntry entityEntry, EntityType entityType)
        {
            var primaryKey = entityType.KeyProperties.First();
            var property = entityEntry.Property(primaryKey.Name);
            return (property.GetNewValue() ?? property.GetOriginalValue())?.ToJsonString();
        }

        [CanBeNull]
        private EntityChange CreateEntityChange(DbEntityEntry entityEntry, EntityType entityType)
        {
            var entityId = GetEntityId(entityEntry, entityType);
            var entityTypeFullName = entityEntry.GetEntityBaseType().FullName;

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
                    changeType = entityEntry.IsDeleted() ? EntityChangeType.Deleted : EntityChangeType.Updated;
                    break;
                case EntityState.Detached:
                case EntityState.Unchanged:
                    Logger.DebugFormat("Skipping Entity Change Creation for {0}, Id:{1}", entityTypeFullName, entityId);
                    return null;
                default:
                    Logger.ErrorFormat("Unexpected {0} - {1}", nameof(entityEntry.State), entityEntry.State);
                    return null;
            }

            if (entityId == null && changeType != EntityChangeType.Created)
            {
                Logger.ErrorFormat("EntityChangeType {0} must have non-empty entity id", changeType);
                return null;
            }

            return new EntityChange
            {
                ChangeType = changeType,
                EntityEntry = entityEntry, // [NotMapped]
                EntityId = entityId,
                EntityTypeFullName = entityTypeFullName,
                TenantId = AbpSession.TenantId
            };
        }

        private EntityType GetEntityType(ObjectContext context, Type entityType, bool useClrType = true)
        {
            var metadataWorkspace = context.MetadataWorkspace;
            if (useClrType)
            {
                /* Get the mapping between Clr types in OSpace */
                var objectItemCollection = ((ObjectItemCollection)metadataWorkspace.GetItemCollection(DataSpace.OSpace));
                return metadataWorkspace
                    .GetItems<EntityType>(DataSpace.OSpace)
                    .Single(e => objectItemCollection.GetClrType(e) == entityType);
            }
            else
            {
                return metadataWorkspace
                    .GetItems<EntityType>(DataSpace.CSpace)
                    .Single(e => e.Name == entityType.Name);
            }
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

        /// <summary>
        /// Gets the property changes for this entry.
        /// </summary>
        private ICollection<EntityPropertyChange> GetPropertyChanges(DbEntityEntry entityEntry, EntityType entityType, EntitySet entitySet, bool auditedPropertiesOnly)
        {
            var propertyChanges = new List<EntityPropertyChange>();

            foreach (var property in entityType.Properties)
            {
                if (entityType.KeyProperties.Any(m => m.Name == property.Name))
                {
                    continue;
                }

                if (!(property.IsPrimitiveType || property.IsComplexType))
                {
                    // Skipping other types of properties
                    // - Reference navigation properties (DbReferenceEntry)
                    // - Collection navigation properties (DbCollectionEntry)
                    continue;
                }

                var propertyEntry = entityEntry.Property(property.Name);
                var propertyInfo = propertyEntry.EntityEntry.GetPropertyInfo(propertyEntry.Name);
                var shouldSaveProperty = IsAuditedPropertyInfo(propertyInfo) ?? !auditedPropertiesOnly;
                if (shouldSaveProperty)
                {
                    propertyChanges.Add(
                        CreateEntityPropertyChange(
                            propertyEntry.GetOriginalValue(),
                            propertyEntry.GetNewValue(),
                            propertyInfo
                        )
                    );
                }
            }

            return propertyChanges;
        }

        /// <summary>
        /// Gets the property changes for this entry.
        /// </summary>
        private ICollection<EntityPropertyChange> GetRelationshipChanges(DbEntityEntry entityEntry, EntityType entityType, EntitySet entitySet, ICollection<ObjectStateEntry> relationshipChanges, bool auditedPropertiesOnly)
        {
            var propertyChanges = new List<EntityPropertyChange>();
            var navigationProperties = entityType.NavigationProperties;

            var isCreated = entityEntry.IsCreated();
            var isDeleted = entityEntry.IsDeleted();

            // Filter out relationship changes that are irrelevant to current entry
            var entityRelationshipChanges = relationshipChanges
                .Where(change => change.EntitySet is AssociationSet)
                .Where(change => change.EntitySet.As<AssociationSet>()
                    .AssociationSetEnds
                    .Select(set => set.EntitySet.ElementType.FullName).Contains(entitySet.ElementType.FullName)
                )
                .ToList();

            var relationshipGroups = entityRelationshipChanges
                .SelectMany(change =>
                {
                    var values = change.State == EntityState.Added ? change.CurrentValues : change.OriginalValues;
                    var valuesChangeSet = new object[values.FieldCount];
                    values.GetValues(valuesChangeSet);

                    return valuesChangeSet
                        .Select(value => value.As<EntityKey>())
                        .Where(value => value.EntitySetName != entitySet.Name)
                        .Select(value => new Tuple<string, EntityState, EntityKey>(change.EntitySet.Name, change.State, value));
                })
                .GroupBy(t => t.Item1);

            foreach (var relationship in relationshipGroups)
            {
                var relationshipName = relationship.Key;
                var navigationProperty = navigationProperties
                    .Where(p => p.RelationshipType.Name == relationshipName)
                    .FirstOrDefault();

                if (navigationProperty == null)
                {
                    Logger.ErrorFormat("Unable to find navigation property for relationship {0} in entity {1}", relationshipName, entityType.Name);
                    continue;
                }

                var propertyInfo = entityEntry.GetPropertyInfo(navigationProperty.Name);
                var shouldSaveProperty = IsAuditedPropertyInfo(propertyInfo) ?? !auditedPropertiesOnly;
                if (shouldSaveProperty)
                {
                    var addedRelationship = relationship.FirstOrDefault(p => p.Item2 == EntityState.Added);
                    var deletedRelationship = relationship.FirstOrDefault(p => p.Item2 == EntityState.Deleted);
                    var newValue = addedRelationship?.Item3.EntityKeyValues.ToDictionary(keyValue => keyValue.Key, keyValue => keyValue.Value);
                    var oldValue = deletedRelationship?.Item3.EntityKeyValues.ToDictionary(keyValue => keyValue.Key, keyValue => keyValue.Value);

                    propertyChanges.Add(CreateEntityPropertyChange(oldValue, newValue, propertyInfo));
                }
            }

            return propertyChanges;
        }

        /// <summary>
        /// Updates change time, entity id, Adds foreign keys, Removes/Updates property changes after SaveChanges is called.
        /// </summary>
        private void UpdateChangeSet(DbContext context, EntityChangeSet changeSet)
        {
            var entityChangesToRemove = new List<EntityChange>();
            foreach (var entityChange in changeSet.EntityChanges)
            {
                var objectContext = context.As<IObjectContextAdapter>().ObjectContext;
                var entityEntry = entityChange.EntityEntry.As<DbEntityEntry>();
                var typeOfEntity = entityEntry.GetEntityBaseType();
                var isAuditedEntity = IsTypeOfAuditedEntity(typeOfEntity) == true;

                /* Update change time */
                entityChange.ChangeTime = GetChangeTime(entityChange.ChangeType, entityEntry.Entity);

                /* Update entity id */
                var entityType = GetEntityType(objectContext, typeOfEntity, useClrType: false);
                entityChange.EntityId = GetEntityId(entityEntry, entityType);

                /* Update property changes */
                var trackedPropertyNames = entityChange.PropertyChanges.Select(pc => pc.PropertyName);
                var trackedNavigationProperties = entityType.NavigationProperties
                                                    .Where(np => trackedPropertyNames.Contains(np.Name))
                                                    .ToList();
                var additionalForeignKeys = trackedNavigationProperties
                                                  .SelectMany(p => p.GetDependentProperties())
                                                  .Where(p => !trackedPropertyNames.Contains(p.Name))
                                                  .Distinct()
                                                  .ToList();

                /* Add additional foreign keys from navigation properties */
                foreach (var foreignKey in additionalForeignKeys)
                {
                    var propertyEntry = entityEntry.Property(foreignKey.Name);
                    var propertyInfo = entityEntry.GetPropertyInfo(foreignKey.Name);

                    var shouldSaveProperty = IsAuditedPropertyInfo(propertyInfo);
                    if (shouldSaveProperty.HasValue && !shouldSaveProperty.Value)
                    {
                        continue;
                    }

                    // TODO: fix new value comparison before truncation
                    var newValue = propertyEntry.GetNewValue()?.ToJsonString().TruncateWithPostfix(EntityPropertyChange.MaxValueLength);
                    var oldValue = propertyEntry.GetOriginalValue()?.ToJsonString().TruncateWithPostfix(EntityPropertyChange.MaxValueLength);

                    // Add foreign key
                    entityChange.PropertyChanges.Add(CreateEntityPropertyChange(oldValue, newValue, propertyInfo));
                }

                /* Update/Remove property changes */
                var propertyChangesToRemove = new List<EntityPropertyChange>();
                foreach (var propertyChange in entityChange.PropertyChanges)
                {
                    var memberEntry = entityEntry.Member(propertyChange.PropertyName);
                    if (!(memberEntry is DbPropertyEntry))
                    {
                        // Skipping other types of properties
                        // - Reference navigation properties (DbReferenceEntry)
                        // - Collection navigation properties (DbCollectionEntry)
                        continue;
                    }

                    var propertyEntry = memberEntry.As<DbPropertyEntry>();
                    var propertyInfo = entityEntry.GetPropertyInfo(propertyChange.PropertyName);
                    var isAuditedProperty = IsAuditedPropertyInfo(propertyInfo) == true;

                    // TODO: fix new value comparison before truncation
                    propertyChange.NewValue = propertyEntry.GetNewValue()?.ToJsonString().TruncateWithPostfix(EntityPropertyChange.MaxValueLength);
                    if (!isAuditedProperty && propertyChange.OriginalValue == propertyChange.NewValue)
                    {
                        // No change
                        propertyChangesToRemove.Add(propertyChange);
                    }
                }

                foreach (var propertyChange in propertyChangesToRemove)
                {
                    entityChange.PropertyChanges.Remove(propertyChange);
                }

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

        private EntityPropertyChange CreateEntityPropertyChange(object oldValue, object newValue, PropertyInfo propertyInfo)
        {
            return new EntityPropertyChange()
            {
                OriginalValue = oldValue?.ToJsonString().TruncateWithPostfix(EntityPropertyChange.MaxValueLength),
                NewValue = newValue?.ToJsonString().TruncateWithPostfix(EntityPropertyChange.MaxValueLength),
                PropertyName = propertyInfo.Name.TruncateWithPostfix(EntityPropertyChange.MaxPropertyNameLength),
                PropertyTypeFullName = propertyInfo.PropertyType.FullName.TruncateWithPostfix(EntityPropertyChange.MaxPropertyTypeFullNameLength),
                TenantId = AbpSession.TenantId
            };
        }
    }
}
