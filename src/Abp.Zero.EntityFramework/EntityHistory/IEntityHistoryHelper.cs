using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.Threading;

namespace Abp.EntityHistory
{
    public interface IEntityHistoryHelper
    {
        EntityChangeSet CreateEntityChangeSet(DbContext context);

        Task SaveAsync(DbContext context, EntityChangeSet changeSet);

        Task<EntityHistorySnapshot> GetEntitySnapshotAsync<TEntity, TPrimaryKey>(TPrimaryKey id, DateTime snapShotTime) where TEntity : class, IEntity<TPrimaryKey>;

        Task<EntityHistorySnapshot> GetEntitySnapshotAsync<TEntity>(int id, DateTime snapShotTime) where TEntity : class, IEntity<int>;
    }

    public static class EntityHistoryHelperExtensions
    {
        public static void Save(this IEntityHistoryHelper entityHistoryHelper, DbContext context, EntityChangeSet changeSet)
        {
            AsyncHelper.RunSync(() => entityHistoryHelper.SaveAsync(context, changeSet));
        }
    }
}
