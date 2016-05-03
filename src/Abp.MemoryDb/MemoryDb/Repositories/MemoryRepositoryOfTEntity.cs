using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using System;

namespace Abp.MemoryDb.Repositories
{
    public class MemoryRepository<TEntity> : MemoryRepository<TEntity, Guid>, IRepository<TEntity>
        where TEntity : class, IEntity<Guid>
    {
        public MemoryRepository(IMemoryDatabaseProvider databaseProvider)
            : base(databaseProvider)
        {
        }
    }
}