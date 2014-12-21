using Abp.Domain.Entities;
using Abp.Domain.Repositories;

namespace Abp.NHibernate.Repositories
{
    /// <summary>
    /// A shortcut of <see cref="NhRepositoryBase{TEntity,TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public class NhRepositoryBase<TEntity> : NhRepositoryBase<TEntity, int>, IRepository<TEntity> where TEntity : class, IEntity<int>
    {
        public NhRepositoryBase(ISessionProvider sessionProvider)
            : base(sessionProvider)
        {
        }
    }
}