using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Entities;
using Abp.Exceptions;
using NHibernate;
using NHibernate.Linq;

namespace Abp.Data.Repositories.NHibernate
{
    /// <summary>
    /// A shortcut of <see cref="NhRepositoryBase{TEntity,TPrimaryKey}"/> for most used primary key type (Int32).
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public class NhRepositoryBase<TEntity> : NhRepositoryBase<TEntity, int>, IRepository<TEntity> where TEntity : class, IEntity<int>
    {

    }

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
        protected ISession Session { get { return NhUnitOfWork.Current.Session; } }

        /// <summary>
        /// Used to get a IQueryable that is used to retrive entities from entire table.
        /// UnitOfWork attrbute must be used to be able to call this method since this method
        /// returns IQueryable and it requires open database connection to use it.
        /// </summary>
        /// <returns>IQueryable to be used to select entities from database</returns>
        public virtual IQueryable<TEntity> GetAll()
        {
            var allEntities = Session.Query<TEntity>();

            //TODO: Move this code out of repository!
            //if ((typeof(IHasTenant)).IsAssignableFrom(typeof(TEntity)) && Tenant.Current != null)
            //{
            //    allEntities = allEntities.Cast<IHasTenant>().Where(entity => entity.Tenant.Id == Tenant.Current.Id).Cast<TEntity>();
            //}

            return allEntities;
        }

        /// <summary>
        /// Used to get all entities.
        /// </summary>
        /// <returns>List of all entities</returns>
        public virtual IList<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        /// <summary>
        /// Used to run a query over entire entities.
        /// UnitOfWork attribute is not always necessery (as opposite to <see cref="GetAll"/>)
        /// if <see cref="queryMethod"/> finishes IQueryable with ToList, FirstOrDefault etc..
        /// </summary>
        /// <typeparam name="T">Type of return value of this method</typeparam>
        /// <param name="queryMethod">This method is used to query over entities</param>
        /// <returns>Query result</returns>
        public virtual T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return queryMethod(GetAll());
        }

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="key">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        public virtual TEntity Get(TPrimaryKey key)
        {
            var entity = GetOrNull(key);
            if (entity == null)
            {
                throw new AbpException("Threre is no such an entity with given primary key. Entity type: " + typeof(TEntity).FullName + ", primary key: " + key);
            }

            return entity;
        }

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="key">Primary key of the entity to get</param>
        /// <returns>Entity or null</returns>
        public TEntity GetOrNull(TPrimaryKey key)
        {
            return Session.Get<TEntity>(key);
        }

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Insert(TEntity entity)
        {
            //TODO: Move this code out of repository!
            //if ((typeof(IHasTenant)).IsAssignableFrom(typeof(TEntity)) && Tenant.Current != null)
            //{
            //    entity.As<IHasTenant>().Tenant = Tenant.Current;
            //}

            Session.Save(entity);
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        public virtual void Update(TEntity entity)
        {
            Session.Update(entity);
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public virtual void Delete(TEntity entity)
        {
            Session.Delete(entity);
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        public virtual void Delete(TPrimaryKey id)
        {
            Session.Delete(Session.Load<TEntity>(id));
        }

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        public int Count()
        {
            return GetAll().Count();
        }

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <param name="queryMethod">A filter method to get count fo a projection</param>
        /// <returns>Count of entities</returns>
        public int Count(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryMethod)
        {
            return queryMethod(GetAll()).Count();
        }

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        public long LongCount()
        {
            return GetAll().LongCount();
        }

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <param name="queryMethod">A filter method to get count fo a projection</param>
        /// <returns>Count of entities</returns>
        public long LongCount(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryMethod)
        {
            return queryMethod(GetAll()).LongCount();
        }
    }
}