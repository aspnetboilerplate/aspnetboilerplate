using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace Reation.CMS.EntityFramework.Repositories
{
    public abstract class CMSRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<CMSDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected CMSRepositoryBase(IDbContextProvider<CMSDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class CMSRepositoryBase<TEntity> : CMSRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected CMSRepositoryBase(IDbContextProvider<CMSDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
