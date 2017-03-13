using System.Data.Entity;
using Abp.Domain.Entities;
using Abp.EntityFramework;

namespace Abp.Dapper.Repositories
{
    public class DapperRepositoryBase<TDbContext, TEntity> : DapperRepositoryBase<TDbContext, TEntity, int>, IDapperRepository<TEntity>
        where TEntity : class, IEntity<int>
        where TDbContext : DbContext
    {
        public DapperRepositoryBase(IDbContextProvider<TDbContext> dbContextProvider) : base(dbContextProvider)
        {
        }
    }
}
