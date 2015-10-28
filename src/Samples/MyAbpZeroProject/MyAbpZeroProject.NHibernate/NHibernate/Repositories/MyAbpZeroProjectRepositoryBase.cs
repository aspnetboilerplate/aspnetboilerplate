using Abp.Domain.Entities;
using Abp.NHibernate;
using Abp.NHibernate.Repositories;

namespace MyAbpZeroProject.NHibernate.Repositories
{
    /// <summary>
    /// Base class for all repositories in this application
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Type of the primary key</typeparam>
    public abstract class MyAbpZeroProjectRepositoryBase<TEntity, TPrimaryKey> : NhRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        protected MyAbpZeroProjectRepositoryBase(ISessionProvider sessionProvider) : base(sessionProvider)
        {
        }

        //add common methods for all repositories
    }

    /// <summary>
    /// A shortcut of MyAbpZeroProjectRepositoryBase for entities with integer Id.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public abstract class MyAbpZeroProjectRepositoryBase<TEntity> : MyAbpZeroProjectRepositoryBase<TEntity, int>
        where TEntity : class, IEntity<int>
    {
        protected MyAbpZeroProjectRepositoryBase(ISessionProvider sessionProvider) : base(sessionProvider)
        {
        }

        //do not add any method here, add to the class above (since this inherits it)
    }
}
