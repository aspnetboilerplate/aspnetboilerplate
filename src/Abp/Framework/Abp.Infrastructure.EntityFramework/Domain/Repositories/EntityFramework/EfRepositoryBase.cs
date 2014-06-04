using System.Data.Entity;
using Abp.Domain.Entities;

namespace Abp.Domain.Repositories.EntityFramework
{
    public abstract class EfRepositoryBase<TDbContext, TEntity> : EfRepositoryBase<TDbContext, TEntity, int>, IRepository<TEntity> where TEntity : class, IEntity<int>
        where TDbContext : DbContext
    {

    }
}