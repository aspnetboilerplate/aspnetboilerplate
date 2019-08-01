using System;
using System.Linq;
using System.Threading.Tasks;
using Abp.Domain.Repositories;
using Abp.EntityFrameworkCore.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Json;
using Abp.Linq;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityHistory
{
    public class EntitySnapshotManager : EntitySnapshotManagerBase
    {
        public EntitySnapshotManager(IRepository<EntityChange, long> entityChangeRepository)
            : base(entityChangeRepository)
        {
        }
        protected override Task<TEntity> GetEntityById<TEntity, TPrimaryKey>(TPrimaryKey id)
        {
            return EntityChangeRepository.GetDbContext()
                .Set<TEntity>().AsQueryable().FirstOrDefaultAsync(CreateEqualityExpressionForId<TEntity, TPrimaryKey>(id));
        }

        protected override IQueryable<EntityChange> GetEntityChanges<TEntity, TPrimaryKey>(TPrimaryKey id, DateTime snapshotTime)
        {
            string fullName = typeof(TEntity).FullName;
            var idJson = id.ToJsonString();

            return EntityChangeRepository.GetAll() //select all changes which created after snapshot time 
                .Where(x => x.EntityTypeFullName == fullName && x.EntityId == idJson && x.ChangeTime > snapshotTime &&
                            x.ChangeType != EntityChangeType.Created)
                .OrderByDescending(x => x.ChangeTime);
        }
    }
}
