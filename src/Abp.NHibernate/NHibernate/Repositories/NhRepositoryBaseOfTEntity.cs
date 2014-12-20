using System;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using NHibernate;

namespace Abp.NHibernate.Repositories
{
    /// <summary>
    /// A shortcut of <see cref="NhRepositoryBase{TEntity,TPrimaryKey}"/> for most used primary key type (<see cref="int"/>).
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public class NhRepositoryBase<TEntity> : NhRepositoryBase<TEntity, int>, IRepository<TEntity> where TEntity : class, IEntity<int>
    {
        public NhRepositoryBase(IUowManager uowManager)
            : base(uowManager)
        {
        }

        public NhRepositoryBase(Func<ISession> sessionFactory)
            : base(sessionFactory)
        {
        }
    }
}