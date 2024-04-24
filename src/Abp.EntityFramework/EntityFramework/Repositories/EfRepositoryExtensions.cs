using System;
using System.Data.Entity;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Reflection;

namespace Abp.EntityFramework.Repositories
{
    public static class EfRepositoryExtensions
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

        public static async Task<DbContext> GetDbContextAsync<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            if (ProxyHelper.UnProxy(repository) is IRepositoryWithDbContext repositoryWithDbContext)
            {
                return await repositoryWithDbContext.GetDbContextAsync();
            }

            throw new ArgumentException("Given repository does not implement IRepositoryWithDbContext", "repository");
        }

        public static void DetachFromDbContext<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            repository.GetDbContext().Entry(entity).State = EntityState.Detached;
        }
    }
}