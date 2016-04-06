using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;

namespace Abp.MemoryDb.Repositories
{
    //TODO: Implement thread-safety..?
    public class MemoryRepository<TEntity, TPrimaryKey> : AbpRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public virtual MemoryDatabase Database { get { return _databaseProvider.Database; } }

        public virtual List<TEntity> Table { get { return Database.Set<TEntity>(); } }

        private readonly IMemoryDatabaseProvider _databaseProvider;
        private readonly MemoryPrimaryKeyGenerator<TPrimaryKey> _primaryKeyGenerator;

        public MemoryRepository(IMemoryDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
            _primaryKeyGenerator = new MemoryPrimaryKeyGenerator<TPrimaryKey>();
        }

        public override IQueryable<TEntity> GetAll()
        {
            return Table.AsQueryable();
        }

        public override TEntity Insert(TEntity entity)
        {
            if (entity.IsTransient())
            {
                entity.Id = _primaryKeyGenerator.GetNext();
            }

            Table.Add(entity);
            return entity;
        }

        public override TEntity Update(TEntity entity)
        {
            var index = Table.FindIndex(e => EqualityComparer<TPrimaryKey>.Default.Equals(e.Id, entity.Id));
            if (index >= 0)
            {
                Table[index] = entity;
            }

            return entity;
        }

        public override void Delete(TEntity entity)
        {
            Delete(entity.Id);
        }

        public override void Delete(TPrimaryKey id)
        {
            var index = Table.FindIndex(e => EqualityComparer<TPrimaryKey>.Default.Equals(e.Id, id));
            if (index >= 0)
            {
                Table.RemoveAt(index);
            }
        }
    }
}