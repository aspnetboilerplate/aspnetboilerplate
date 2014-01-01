using System;
using System.Collections.Generic;
using System.Linq;
using Abp.Domain.Entities;

namespace Abp.Domain.Repositories
{
    /// <summary>
    /// This interface is implemented by all repositories to ensure implementation of fixed methods.
    /// TODO: Add method overloads for most used cases.
    /// </summary>
    /// <typeparam name="TEntity">Main Entity type this repository works on</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public interface IRepository<TEntity, in TPrimaryKey> : IRepository where TEntity : IEntity<TPrimaryKey>
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
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="key">Primary key of the entity to get</param>
        /// <returns>Entity or null</returns>
        TEntity GetOrNull(TPrimaryKey key);

        /// <summary>
        /// Creates an entity with given primary key without database access.
        /// </summary>
        /// <param name="key">Primary key of the entity to load</param>
        /// <returns>Entity</returns>
        TEntity Load(TPrimaryKey key);

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

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        int Count();

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <param name="queryMethod">A filter method to get count fo a projection</param>
        /// <returns>Count of entities</returns>
        int Count(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryMethod);

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        long LongCount();

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <param name="queryMethod">A filter method to get count fo a projection</param>
        /// <returns>Count of entities</returns>
        long LongCount(Func<IQueryable<TEntity>, IQueryable<TEntity>> queryMethod);
    }
}
