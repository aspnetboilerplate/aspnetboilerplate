using System;
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
            var repositoryWithDbContext = repository as IRepositoryWithDbContext;
            if (repositoryWithDbContext != null)
            {
                return repositoryWithDbContext.GetDbContext();
            }

            var targetWithDbContext = CastleProxyHelper.GetProxyTargetOrNull(repository) as IRepositoryWithDbContext;
            if (targetWithDbContext != null)
            {
                return targetWithDbContext.GetDbContext();
            }

            throw new ArgumentException("Given repository does not implement IRepositoryWithDbContext", nameof(repository));
        }

        public static void DetachFromDbContext<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository, TEntity entity)
            where TEntity : class, IEntity<TPrimaryKey>
        {
            repository.GetDbContext().Entry(entity).State = EntityState.Detached;
        }
    }
}