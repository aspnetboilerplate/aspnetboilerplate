using Abp.Domain.Entities;
using Abp.EntityFramework;
using Abp.EntityFramework.Repositories;

namespace MyAbpZeroProject.EntityFramework.Repositories
{
    public abstract class MyAbpZeroProjectRepositoryBase<TEntity, TPrimaryKey> : EfRepositoryBase<MyAbpZeroProjectDbContext, TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected MyAbpZeroProjectRepositoryBase(IDbContextProvider<MyAbpZeroProjectDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //add common methods for all repositories
    }

    public abstract class MyAbpZeroProjectRepositoryBase<TEntity> : MyAbpZeroProjectRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected MyAbpZeroProjectRepositoryBase(IDbContextProvider<MyAbpZeroProjectDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
