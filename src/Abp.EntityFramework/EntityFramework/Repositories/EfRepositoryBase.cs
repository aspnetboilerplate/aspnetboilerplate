using System;
using System.Data.Entity;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

namespace Abp.EntityFramework.Repositories
{
    public class EfRepositoryBase<TDbContext, TEntity> : EfRepositoryBase<TDbContext, TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext
    {
        public EfRepositoryBase(IUowManager uowManager)
            : base(uowManager)
        {

        }

        public EfRepositoryBase(Func<TDbContext> dbContextFactory)
            : base(dbContextFactory)
        {

        }
    }
}