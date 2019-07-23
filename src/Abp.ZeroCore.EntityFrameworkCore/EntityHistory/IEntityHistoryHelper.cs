using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.Threading;

namespace Abp.EntityHistory
{
    public interface IEntityHistoryHelper
    {
        EntityChangeSet CreateEntityChangeSet(ICollection<EntityEntry> entityEntries);

        Task SaveAsync(EntityChangeSet changeSet);

        Task<EntityHistorySnapshot> GetEntitySnapshotAsync<TEntity, TPrimaryKey>(TPrimaryKey id, DateTime snapShotTime) where TEntity : Entity<TPrimaryKey>;

        Task<EntityHistorySnapshot> GetEntitySnapshotAsync<TEntity>(int id, DateTime snapShotTime) where TEntity : Entity<int>;
    }

    public static class EntityHistoryHelperExtensions
    {
        public static void Save(this IEntityHistoryHelper entityHistoryHelper, EntityChangeSet changeSet)
        {
            AsyncHelper.RunSync(() => entityHistoryHelper.SaveAsync(changeSet));
        }
    }
}
