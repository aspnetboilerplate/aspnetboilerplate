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
    }

    public static class EntityHistoryHelperExtensions
    {
        public static void Save(this IEntityHistoryHelper entityHistoryHelper, EntityChangeSet changeSet)
        {
            AsyncHelper.RunSync(() => entityHistoryHelper.SaveAsync(changeSet));
        }
    }
}
