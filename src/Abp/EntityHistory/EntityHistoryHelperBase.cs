using System;
using System.Linq;
using System.Reflection;
using Abp.Auditing;
using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Domain.Uow;
using Abp.Events.Bus.Entities;
using Abp.Runtime.Session;
using Abp.Timing;
using Castle.Core.Logging;

namespace Abp.EntityHistory
{
    public abstract class EntityHistoryHelperBase
    {
        public ILogger Logger { get; set; }
        public IAbpSession AbpSession { get; set; }
        public IClientInfoProvider ClientInfoProvider { get; set; }
        public IEntityChangeSetReasonProvider EntityChangeSetReasonProvider { get; set; }
        public IEntityHistoryStore EntityHistoryStore { get; set; }

        protected readonly IEntityHistoryConfiguration EntityHistoryConfiguration;
        protected readonly IUnitOfWorkManager UnitOfWorkManager;

        protected bool IsEntityHistoryEnabled => EntityHistoryConfiguration.IsEnabled &&
            (AbpSession.UserId.HasValue || EntityHistoryConfiguration.IsEnabledForAnonymousUsers);

        protected EntityHistoryHelperBase(
            IEntityHistoryConfiguration entityHistoryConfiguration,
            IUnitOfWorkManager unitOfWorkManager)
        {
            EntityHistoryConfiguration = entityHistoryConfiguration;
            UnitOfWorkManager = unitOfWorkManager;

            AbpSession = NullAbpSession.Instance;
            Logger = NullLogger.Instance;
            ClientInfoProvider = NullClientInfoProvider.Instance;
            EntityChangeSetReasonProvider = NullEntityChangeSetReasonProvider.Instance;
            EntityHistoryStore = NullEntityHistoryStore.Instance;
        }

        protected virtual DateTime GetChangeTime(EntityChangeType entityChangeType, object entity)
        {
            switch (entityChangeType)
            {
                case EntityChangeType.Created:
                    return (entity as IHasCreationTime)?.CreationTime ?? Clock.Now;
                case EntityChangeType.Deleted:
                    return (entity as IHasDeletionTime)?.DeletionTime ?? Clock.Now;
                case EntityChangeType.Updated:
                    return (entity as IHasModificationTime)?.LastModificationTime ?? Clock.Now;
                default:
                    Logger.ErrorFormat("Unexpected {0} - {1}", nameof(entityChangeType), entityChangeType);
                    return Clock.Now;
            }
        }

        protected virtual bool IsTypeOfEntity(Type entityType)
        {
            return EntityHelper.IsEntity(entityType) && entityType.IsPublic;
        }

        protected virtual bool? IsTypeOfAuditedEntity(Type entityType)
        {
            var entityTypeInfo = entityType.GetTypeInfo();
            if (entityTypeInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            if (entityTypeInfo.IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            return null;
        }

        protected virtual bool? IsTypeOfTrackedEntity(Type entityType)
        {
            if (EntityHistoryConfiguration.IgnoredTypes.Any(type => type.GetTypeInfo().IsAssignableFrom(entityType)))
            {
                return false;
            }

            if (EntityHistoryConfiguration.Selectors.Any(selector => selector.Predicate(entityType)))
            {
                return true;
            }

            return null;
        }

        protected virtual bool? IsAuditedPropertyInfo(PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsDefined(typeof(DisableAuditingAttribute), true))
            {
                return false;
            }

            if (propertyInfo.IsDefined(typeof(AuditedAttribute), true))
            {
                return true;
            }

            return null;
        }
    }
}
