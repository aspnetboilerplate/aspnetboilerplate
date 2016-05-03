using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using System;
using System.Data.Entity;

namespace Abp.EntityFramework.Repositories
{
    public class EfRepositoryBase<TDbContext, TEntity> : EfRepositoryBase<TDbContext, TEntity, Guid>, IRepository<TEntity>
        where TEntity : class, IEntity<Guid>
        where TDbContext : DbContext
    {
        public EfRepositoryBase(IDbContextProvider<TDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}