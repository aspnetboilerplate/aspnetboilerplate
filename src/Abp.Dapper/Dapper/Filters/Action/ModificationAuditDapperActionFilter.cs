using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Extensions;
using Abp.Timing;

namespace Abp.Dapper.Filters.Action
{
    public class ModificationAuditDapperActionFilter : DapperActionFilterBase, IDapperActionFilter
    {
        public void ExecuteFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (entity is IHasModificationTime)
            {
                entity.As<IHasModificationTime>().LastModificationTime = Clock.Now;
            }

            if (entity is IModificationAudited)
            {
                var record = entity.As<IModificationAudited>();
                long? userId = GetAuditUserId();
                if (userId == null)
                {
                    record.LastModifierUserId = null;
                    return;
                }

                //Special check for multi-tenant entities
                if (entity is IMayHaveTenant || entity is IMustHaveTenant)
                {
                    //Sets LastModifierUserId only if current user is in same tenant/host with the given entity
                    if (entity is IMayHaveTenant && entity.As<IMayHaveTenant>().TenantId == AbpSession.TenantId ||
                        entity is IMustHaveTenant && entity.As<IMustHaveTenant>().TenantId == AbpSession.TenantId)
                    {
                        record.LastModifierUserId = userId;
                    }
                    else
                    {
                        record.LastModifierUserId = null;
                    }
                }
                else
                {
                    record.LastModifierUserId = userId;
                }
            }
        }
    }
}
