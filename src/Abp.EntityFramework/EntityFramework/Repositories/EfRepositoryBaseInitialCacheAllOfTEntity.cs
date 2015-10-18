using System.Data.Entity;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFramework;
using Abp.Runtime.Caching;

namespace Abp.EntityFramework.Repositories
{
    public class EfRepositoryBaseInitialCacheAll<TDbContext, TEntity> : EfRepositoryBaseInitialCacheAll<TDbContext, TEntity, int>, IRepository<TEntity>, IFromDB<TEntity>
        where TEntity : class, IEntity<int>
        where TDbContext : AbpDbContext
    {
        public EfRepositoryBaseInitialCacheAll(IDbContextProvider<TDbContext> dbContextProvider, ICacheManager cacheManager)
            : base(dbContextProvider, cacheManager)
        {
        }
    }
}