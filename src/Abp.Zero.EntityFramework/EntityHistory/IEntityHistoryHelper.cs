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

        /// <summary>
        /// Returns snapshot of entity at given snapshotTime.
        /// <para>
        /// The ChangedPropertiesSnapshots values represent the value of the entity at snapShotTime.
        /// The PropertyChangesStackTree values represent the stacktree of changed value from now to snapshotTime.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">typeof entity</typeparam>
        /// <typeparam name="TPrimaryKey">typeof primary key</typeparam>
        /// <param name="id">entity id</param>
        /// <param name="snapshotTime"></param>
        Task<EntityHistorySnapshot> GetEntitySnapshotAsync<TEntity, TPrimaryKey>(TPrimaryKey id, DateTime snapshotTime) where TEntity : class, IEntity<TPrimaryKey>;

        /// <summary>
        /// shortcut of GetEntitySnapshotAsync &lt;TEntity, int &gt;
        /// </summary>
        Task<EntityHistorySnapshot> GetEntitySnapshotAsync<TEntity>(int id, DateTime snapshotTime) where TEntity : class, IEntity<int>;
    }

    public static class EntityHistoryHelperExtensions
    {
        public static void Save(this IEntityHistoryHelper entityHistoryHelper, DbContext context, EntityChangeSet changeSet)
        {
            AsyncHelper.RunSync(() => entityHistoryHelper.SaveAsync(context, changeSet));
        }
    }
}
