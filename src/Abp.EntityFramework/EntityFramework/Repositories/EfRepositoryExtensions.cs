using System;
using System.Data.Entity;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;

namespace Abp.EntityFramework.Repositories
{
    public static class EfRepositoryExtensions
    {
        public static DbContext GetDbContext<TEntity, TPrimaryKey>(this IRepository<TEntity, TPrimaryKey> repository)
            where TEntity : class, IEntity<TPrimaryKey>, new()
        {
            var repositoryWithDbContext = repository as IRepositoryWithDbContext;
            if (repositoryWithDbContext == null)
            {
                throw new ArgumentException("Given repository does not implement IRepositoryWithDbContext", "repository");
            }

            return repositoryWithDbContext.GetDbContext();
        }
    }
}