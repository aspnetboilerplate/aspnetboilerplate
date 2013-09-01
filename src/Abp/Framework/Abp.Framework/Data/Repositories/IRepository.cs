using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Entities;

namespace Abp.Data.Repositories
{
    /// <summary>
    /// This interface must be implemented by all repositories to identify them.
    /// Implement generic version instead of this one.
    /// </summary>
    public interface IRepository
    {

    }

    /// <summary>
    /// A shortcut of <see cref="IRepository{TEntity,TPrimaryKey}"/> for most used primary key type (Int32).
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : IEntity<int>
    {

    }

    /// <summary>
    /// This interface is implemented by all repositories to ensure implementation of fixed methods.
    /// </summary>
    /// <typeparam name="TEntity">Main Entity type this repository works on</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public interface IRepository<TEntity, TPrimaryKey> : IRepository where TEntity : IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Used to get a IQueryable that is used to retrive entities from entire table.
        /// UnitOfWork attrbute must be used to be able to call this method since this method
        /// returns IQueryable and it requires open database connection to use it.
        /// </summary>
        /// <returns>IQueryable to be used to select entities from database</returns>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Used to get all entities.
        /// </summary>
        /// <returns>List of all entities</returns>
        IList<TEntity> GetAllList();

        /// <summary>
        /// Used to run a query over entire entities.
        /// UnitOfWork attribute is not always necessery (as opposite to <see cref="GetAll"/>)
        /// if <see cref="queryMethod"/> finishes IQueryable with ToList, FirstOrDefault etc..
        /// </summary>
        /// <typeparam name="T">Type of return value of this method</typeparam>
        /// <param name="queryMethod">This method is used to query over entities</param>
        /// <returns>Query result</returns>
        T Query<T>(Func<IQueryable<TEntity>, T> queryMethod);

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="key">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        TEntity Get(TPrimaryKey key);

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        void Insert(TEntity entity);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        void Update(TEntity entity);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        void Delete(TEntity entity);

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        void Delete(TPrimaryKey id);

        int Count();

        int Count(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryMethod);

        long LongCount();

        long LongCount(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryMethod);
    }
}