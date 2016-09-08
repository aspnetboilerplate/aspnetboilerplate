using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace Abp.TestBase.SampleApplication.EntityFramework.Repositories
{
    public class SampleApplicationEfRepositoryBase<TEntity> : SampleApplicationEfRepositoryBase<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        public SampleApplicationEfRepositoryBase(IDbContextProvider<SampleApplicationDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }

    public class SampleApplicationEfRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<SampleApplicationDbContext, TEntity, TPrimaryKey>
    where TEntity : class, IEntity<TPrimaryKey>
    {
        public SampleApplicationEfRepositoryBase(IDbContextProvider<SampleApplicationDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }
    }
}