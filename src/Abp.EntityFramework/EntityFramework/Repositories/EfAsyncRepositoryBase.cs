using System.Data.Entity;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;

namespace Abp.EntityFramework.Repositories
{
    public class EfAsyncRepositoryBase<TDbContext, TEntity> : EfAsyncRepositoryBase<TDbContext, TEntity, int>, IAsyncRepository<TEntity>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext
    {

    }
}