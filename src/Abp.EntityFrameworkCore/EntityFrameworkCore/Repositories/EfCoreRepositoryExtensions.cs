using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abp.Reflection;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Abp.EntityFrameworkCore.Repositories
{
    public static class EfCoreRepositoryExtensions
    {
        public static DbContext GetDbContext<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            var repositoryWithDbContext = ProxyHelper.UnProxy(repository) as IRepositoryWithDbContext;
            if (repositoryWithDbContext != null)
            {
                return repositoryWithDbContext.GetDbContext();
            }

            throw new ArgumentException("Given repository does not implement IRepositoryWithDbContext", nameof(repository));
        }

        public static void DetachFromDbContext<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            repository.GetDbContext().Entry(entity).State = EntityState.Detached;
        }
        
        public static void InsertRange<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, params TEntity[] entities) 
            where TEntity : class, IEntity<TPrimaryKey>
        {
            repository.GetDbContext().AddRange(entities.ToArray<object>());
        }
        
        public static void InsertRange<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, IEnumerable<TEntity> entities) 
            where TEntity : class, IEntity<TPrimaryKey>
        {
            repository.GetDbContext().AddRange(entities);
        }
        
        public static async Task InsertRangeAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, params TEntity[] entities) 
            where TEntity : class, IEntity<TPrimaryKey>
        {
            await repository.GetDbContext().AddRangeAsync(entities.ToArray<object>());
        }
        
        public static async Task InsertRangeAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, IEnumerable<TEntity> entities) 
            where TEntity : class, IEntity<TPrimaryKey>
        {
            await repository.GetDbContext().AddRangeAsync(entities);
        }
        
        public static void RemoveRange<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, params TEntity[] entities) 
            where TEntity : class, IEntity<TPrimaryKey>
        {
            repository.GetDbContext().RemoveRange(entities.ToArray<object>());
        }
        
        public static void RemoveRange<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, IEnumerable<TEntity> entities) 
            where TEntity : class, IEntity<TPrimaryKey>
        {
            repository.GetDbContext().RemoveRange(entities);
        }
    }
}