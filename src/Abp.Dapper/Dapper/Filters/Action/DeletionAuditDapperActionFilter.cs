using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Extensions;
using Abp.Timing;

namespace Abp.Dapper.Filters.Action
{
    public class DeletionAuditDapperActionFilter : DapperActionFilterBase, IDapperActionFilter
    {
        public void ExecuteFilter<TEntity, TPrimaryKey>(TEntity entity) where TEntity : class, IEntity<TPrimaryKey>
        {
            if (entity is ISoftDelete)
            {
                var record = entity.As<ISoftDelete>();
                record.IsDeleted = true;
            }

            if (entity is IHasDeletionTime)
            {
                var record = entity.As<IHasDeletionTime>();
                if (record.DeletionTime == null)
                {
                    record.DeletionTime = Clock.Now;
                }
            }

            if (entity is IDeletionAudited)
            {
                long? userId = GetAuditUserId();
                var record = entity.As<IDeletionAudited>();

                if (record.DeleterUserId != null)
                {
                    return;
                }

                if (userId == null)
                {
                    record.DeleterUserId = null;
                    return;
                }

                if (entity is IMayHaveTenant || entity is IMustHaveTenant)
                {
                    //Sets LastModifierUserId only if current user is in same tenant/host with the given entity
                    if (entity is IMayHaveTenant && entity.As<IMayHaveTenant>().TenantId == AbpSession.TenantId ||
                        entity is IMustHaveTenant && entity.As<IMustHaveTenant>().TenantId == AbpSession.TenantId)
                    {
                        record.DeleterUserId = userId;
                    }
                    else
                    {
                        record.DeleterUserId = null;
                    }
                }
                else
                {
                    record.DeleterUserId = userId;
                }
            }
        }
    }
}
