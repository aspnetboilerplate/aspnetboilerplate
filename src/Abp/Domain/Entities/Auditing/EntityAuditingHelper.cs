using Abp.Timing;
using System;
using Abp.Configuration.Startup;
using Abp.Domain.Uow;
using Abp.MultiTenancy;
using Abp.Extensions;

namespace Abp.Domain.Entities.Auditing
{
    public static class EntityAuditingHelper
    {
        public static void SetCreationAuditProperties(
            IMultiTenancyConfig multiTenancyConfig,
            object entityAsObj, 
            int? tenantId,
            long? userId,
            UnitOfWorkAuditingConfiguration auditingConfiguration)
        {
            var entityWithCreationTime = entityAsObj as IHasCreationTime;
            if (entityWithCreationTime == null)
            {
                //Object does not implement IHasCreationTime
                return;
            }

            if (entityWithCreationTime.CreationTime == default)
            {
                entityWithCreationTime.CreationTime = Clock.Now;
            }

            if (!(entityAsObj is ICreationAudited))
            {
                //Object does not implement ICreationAudited
                return;
            }

            if (!userId.HasValue)
            {
                //Unknown user
                return;
            }

            var entity = entityAsObj as ICreationAudited;
            if (entity.CreatorUserId != null)
            {
                //CreatorUserId is already set
                return;
            }

            if (multiTenancyConfig?.IsEnabled == true)
            {
                if (MultiTenancyHelper.IsMultiTenantEntity(entity) &&
                    !MultiTenancyHelper.IsTenantEntity(entity, tenantId))
                {
                    //A tenant entity is created by host or a different tenant
                    return;
                }

                if (tenantId.HasValue && MultiTenancyHelper.IsHostEntity(entity))
                {
                    //Tenant user created a host entity
                    return;
                }
            }

            if (auditingConfiguration.DisableCreatorUserId)
            {
                return;
            }
            
            //Finally, set CreatorUserId!
            entity.CreatorUserId = userId;
        }

        public static void SetModificationAuditProperties(
            IMultiTenancyConfig multiTenancyConfig,
            object entityAsObj,
            int? tenantId,
            long? userId,
            UnitOfWorkAuditingConfiguration auditingConfiguration)
        {
            if (entityAsObj is IHasModificationTime)
            {
                entityAsObj.As<IHasModificationTime>().LastModificationTime = Clock.Now;
            }

            if (!(entityAsObj is IModificationAudited))
            {
                //Entity does not implement IModificationAudited
                return;
            }

            var entity = entityAsObj.As<IModificationAudited>();

            if (userId == null)
            {
                //Unknown user
                entity.LastModifierUserId = null;
                return;
            }

            if (multiTenancyConfig?.IsEnabled == true)
            {
                if (MultiTenancyHelper.IsMultiTenantEntity(entity) &&
                    !MultiTenancyHelper.IsTenantEntity(entity, tenantId))
                {
                    //A tenant entitiy is modified by host or a different tenant
                    entity.LastModifierUserId = null;
                    return;
                }

                if (tenantId.HasValue && MultiTenancyHelper.IsHostEntity(entity))
                {
                    //Tenant user modified a host entity
                    entity.LastModifierUserId = null;
                    return;
                }
            }

            if (auditingConfiguration.DisableLastModifierUserId)
            {
                return;
            }
            
            //Finally, set LastModifierUserId!
            entity.LastModifierUserId = userId;
        }

        public static void SetDeletionAuditProperties(
            IMultiTenancyConfig multiTenancyConfig, 
            object entityAsObj, 
            int? tenantId, 
            long? userId, 
            UnitOfWorkAuditingConfiguration auditingConfiguration)
        {
            if (entityAsObj is IHasDeletionTime)
            {
                var entity = entityAsObj.As<IHasDeletionTime>();

                if (entity.DeletionTime == null)
                {
                    entity.DeletionTime = Clock.Now;
                }
            }

            if (entityAsObj is IDeletionAudited)
            {
                var entity = entityAsObj.As<IDeletionAudited>();

                if (entity.DeleterUserId != null)
                {
                    return;
                }

                if (userId == null)
                {
                    entity.DeleterUserId = null;
                    return;
                }

                if (auditingConfiguration.DisableDeleterUserId)
                {
                    return;
                }
                
                //Special check for multi-tenant entities
                if (entity is IMayHaveTenant || entity is IMustHaveTenant)
                {
                    //Sets LastModifierUserId only if current user is in same tenant/host with the given entity
                    if ((entity is IMayHaveTenant && entity.As<IMayHaveTenant>().TenantId == tenantId) ||
                        (entity is IMustHaveTenant && entity.As<IMustHaveTenant>().TenantId == tenantId))
                    {
                        entity.DeleterUserId = userId;
                    }
                    else
                    {
                        entity.DeleterUserId = null;
                    }
                }
                else
                {
                    entity.DeleterUserId = userId;
                }
            }
        }
    }
}
