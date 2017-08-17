using Abp.Timing;
using System;
using Abp.Configuration.Startup;
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
            long? userId)
        {
            var entityWithCreationTime = entityAsObj as IHasCreationTime;
            if (entityWithCreationTime == null)
            {
                //Object does not implement IHasCreationTime
                return;
            }

            if (entityWithCreationTime.CreationTime == default(DateTime))
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
                    //A tenant entitiy is created by host or a different tenant
                    return;
                }

                if (tenantId.HasValue && MultiTenancyHelper.IsHostEntity(entity))
                {
                    //Tenant user created a host entity
                    return;
                }
            }

            //Finally, set CreatorUserId!
            entity.CreatorUserId = userId;
        }

        public static void SetModificationAuditProperties(
            IMultiTenancyConfig multiTenancyConfig,
            object entityAsObj,
            int? tenantId,
            long? userId)
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

            //Finally, set LastModifierUserId!
            entity.LastModifierUserId = userId;
        }
    }
}
