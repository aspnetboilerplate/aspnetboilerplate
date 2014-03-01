using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Exceptions;
using NHibernate;
using NHibernate.Linq;

namespace Abp.Domain.Repositories.NHibernate
{
    /// <summary>
    /// Base class for all repositories those uses NHibernate.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public class NhRepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Gets the NHibernate session object to perform database operations.
        /// </summary>
        protected ISession Session { get { return ((NhUnitOfWork)UnitOfWorkScope.CurrentUow).Session; } }

        public virtual IQueryable<TEntity> GetAll()
        {
            return Session.Query<TEntity>();
        }

        public virtual List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        public virtual List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public virtual T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return queryMethod(GetAll());
        }

        public virtual TEntity Get(TPrimaryKey key)
        {
            var entity = FirstOrDefault(key);
            if (entity == null)
            {
                throw new AbpException("Threre is no such an entity with given primary key. Entity type: " + typeof(TEntity).FullName + ", primary key: " + key);
            }

            return entity;
        }

        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Single(predicate);
        }

        public virtual TEntity FirstOrDefault(TPrimaryKey key)
        {
            return Session.Get<TEntity>(key);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public virtual TEntity Load(TPrimaryKey key)
        {
            return Session.Load<TEntity>(key);
        }

        public virtual void Insert(TEntity entity)
        {
            Session.Save(entity);
        }

        public virtual void Update(TEntity entity)
        {
            Session.Update(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            Session.Delete(entity);
        }

        public virtual void Delete(TPrimaryKey id)
        {
            Session.Delete(Session.Load<TEntity>(id));
        }

        public virtual int Count()
        {
            return GetAll().Count();
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).Count();
        }

        public virtual long LongCount()
        {
            return GetAll().LongCount();
        }

        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).LongCount();
        }
    }
}
