using System;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Abp.EntityHistory
{
    public static class EntitySnapshotManagerExtensions
    {
        /// <summary>
        /// shortcut of (IEntitySnapshotManager).GetEntitySnapshotAsync &lt;TEntity, int &gt;
        /// </summary>
        public static async Task<EntityHistorySnapshot> GetSnapshotAsync<TEntity>(
            this IEntitySnapshotManager entitySnapshotManager, 
            int id, 
            DateTime snapshotTime)
            where TEntity : class, IEntity<int>
        {
            return await entitySnapshotManager.GetSnapshotAsync<TEntity, int>(id, snapshotTime);
        }
    }
}
