using System;
using System.Collections.Generic;
using System.Linq;
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
            var changeSet = new EntityChangeSet();

            foreach (var entry in entityEntries)
            {
                changeSet.EntityChanges.Add(CreateEntityChangeInfo(entry));
            }

            return changeSet;
        }

        public bool ShouldSaveEntityHistory(EntityEntry entityEntry, bool defaultValue = false)
        {
            if (!_configuration.IsEnabled)
            {
                return false;
            }

            if (!_configuration.IsEnabledForAnonymousUsers && (AbpSession?.UserId == null))
            {
                return false;
            }

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

            if (entityType.IsDefined(typeof(EntityChangeTrackedAttribute), true))
            {
                return true;
            }

            if (entityType.IsDefined(typeof(DisableEntityChangeTrackingAttribute), true))
            {
                return false;
            }

            if (_configuration.Selectors.Any(selector => selector.Predicate(entityType)))
            {
                return true;
            }

            return defaultValue;
        }

        public async Task SaveAsync(EntityChangeSet changeSet, DbContext context)
        {
            UpdateChangeSet(changeSet, context);

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.RequiresNew))
            {
                await EntityHistoryStore.SaveAsync(changeSet);
                await uow.CompleteAsync();
            }
        }

        public async Task SaveAsync(EntityEntry entityEntry)
        {
            var entityChangeInfo = CreateEntityChangeInfo(entityEntry);
            if (entityChangeInfo == null)
            {
                return;
            }

            using (var uow = _unitOfWorkManager.Begin(TransactionScopeOption.Suppress))
            {
                await EntityHistoryStore.SaveAsync(entityChangeInfo);
                await uow.CompleteAsync();
            }

            // TODO: Save EntityPropertyChange!
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
                    if (entity is ISoftDelete && entity.As<ISoftDelete>().IsDeleted)
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
            if (entityId == null)
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

            foreach (var property in properties)
            {
                var propertyEntry = entityEntry.Property(property.Name);
                if (propertyEntry.IsModified)
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

        private void UpdateChangeSet(EntityChangeSet changeSet, DbContext context)
        {
            throw new NotImplementedException();
        }
    }
}
