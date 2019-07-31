using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;
using Abp.Events.Bus.Entities;
using Abp.Json;

namespace Abp.EntityHistory
{
    public class EntitySnapshotManager : EntitySnapshotManagerBase
    {
        private readonly IRepository<EntityChange, long> _entityChangeRepository;

        public EntitySnapshotManager(IRepository<EntityChange, long> entityChangeRepository)
        {
            _entityChangeRepository = entityChangeRepository;
        }

        protected override async Task<TEntity> GetEntityById<TEntity, TPrimaryKey>(TPrimaryKey id)
        {
            return await _entityChangeRepository.GetDbContext()
                .Set<TEntity>().AsQueryable().FirstOrDefaultAsync(CreateEqualityExpressionForId<TEntity, TPrimaryKey>(id)); ;
        }

        protected override IQueryable<EntityChange> GetEntityChanges<TEntity, TPrimaryKey>(TPrimaryKey id,
            DateTime snapshotTime)
        {
            string fullName = typeof(TEntity).FullName;
            var idJson = id.ToJsonString();

            return _entityChangeRepository.GetAll() //select all changes which created after snapshot time 
                .Where(x => x.EntityTypeFullName == fullName && x.EntityId == idJson && x.ChangeTime > snapshotTime &&
                            x.ChangeType != EntityChangeType.Created)
                .OrderByDescending(x => x.ChangeTime);
        }
    }
}
