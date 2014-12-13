using Abp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.Domain.Repositories
{
    /// <summary>
    /// This interface is implemented by all async repositories to ensure implementation of fixed methods.
    /// </summary>
    /// <typeparam name="TEntity">Main Entity type this repository works on</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public interface IAsyncRepository<TEntity, TPrimaryKey> : IAsyncRepository where TEntity : class, IEntity<TPrimaryKey>
    {
        #region Select/Get/Query

        /// <summary>
        /// Used to get a <see cref="IEnumerable{TEntity}"/> that is used to retrive entities from entire table.
        /// <see cref="UnitOfWorkAttribute"/> attrbute must be used to be able to call this method since this method.
        /// </summary>
        /// <returns><see cref="IEnumerable{TEntity}"/> with entities returned from database</returns>
        Task<IEnumerable<TEntity>> GetAll();

        /// <summary>
        /// Used to get a <see cref="IEnumerable{TEntity}"/> that is used to retrive entities from entire table.
        /// <see cref="UnitOfWorkAttribute"/> attrbute must be used to be able to call this method since this method.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns><see cref="IEnumerable{TEntity}"/> with entities returned from database</returns>
        Task<IEnumerable<TEntity>> GetAll(CancellationToken cancellationToken);

        /// <summary>
        /// Used to get all entities.
        /// </summary>
        /// <returns>List of all entities</returns>
        Task<List<TEntity>> GetAllList();

        /// <summary>
        /// Used to get all entities.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>List of all entities</returns>
        Task<List<TEntity>> GetAllList(CancellationToken cancellationToken);

        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <returns>List of all entities</returns>
        Task<List<TEntity>> GetAllList(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Used to get all entities based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>List of all entities</returns>
        Task<List<TEntity>> GetAllList(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        /// <summary>
        /// Used to run a query over entire entities.
        /// <see cref="UnitOfWorkAttribute"/> attribute is not always necessery (as opposite to <see cref="GetAll"/>)
        /// if <paramref name="queryMethod"/> finishes IQueryable with ToList, FirstOrDefault etc..
        /// </summary>
        /// <typeparam name="T">Type of return value of this method</typeparam>
        /// <param name="queryMethod">This method is used to query over entities</param>
        /// <returns>Query result</returns>
        Task<T> Query<T>(Func<IQueryable<TEntity>, T> queryMethod);

        /// <summary>
        /// Used to run a query over entire entities.
        /// <see cref="UnitOfWorkAttribute"/> attribute is not always necessery (as opposite to <see cref="GetAll"/>)
        /// if <paramref name="queryMethod"/> finishes IQueryable with ToList, FirstOrDefault etc..
        /// </summary>
        /// <typeparam name="T">Type of return value of this method</typeparam>
        /// <param name="queryMethod">This method is used to query over entities</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Query result</returns>
        Task<T> Query<T>(Func<IQueryable<TEntity>, T> queryMethod, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity</returns>
        Task<TEntity> Get(TPrimaryKey id);

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Entity</returns>
        Task<TEntity> Get(TPrimaryKey id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// Throws exception if no entity or more than one entity.
        /// </summary>
        /// <param name="predicate">The predicate to find the entity</param>
        /// <returns>Entity</returns>
        Task<TEntity> Single(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets exactly one entity with given predicate.
        /// Throws exception if no entity or more than one entity.
        /// </summary>
        /// <param name="predicate">The predicate to find the entity</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Entity</returns>
        Task<TEntity> Single(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <returns>Entity or null</returns>
        Task<TEntity> FirstOrDefault(TPrimaryKey id);

        /// <summary>
        /// Gets an entity with given primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity to get</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Entity or null</returns>
        Task<TEntity> FirstOrDefault(TPrimaryKey id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an entity with given given predicate.
        /// </summary>
        /// <param name="predicate">Predicate to filter entities</param>
        /// <returns></returns>
        Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets an entity with given given predicate.
        /// </summary>
        /// <param name="predicate">Predicate to filter entities</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Entity or null</returns>
        Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        /// <summary>
        /// Creates an entity with given primary key without database access.
        /// </summary>
        /// <param name="id">Primary key of the entity to load</param>
        /// <returns>Entity</returns>
        Task<TEntity> Load(TPrimaryKey id);

        /// <summary>
        /// Creates an entity with given primary key without database access.
        /// </summary>
        /// <param name="id">Primary key of the entity to load</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Entity</returns>
        Task<TEntity> Load(TPrimaryKey id, CancellationToken cancellationToken);

        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        Task<TEntity> Insert(TEntity entity);

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <param name="entity">Entity</param>
        Task<TEntity> Insert(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        Task<TPrimaryKey> InsertAndGetId(TEntity entity);

        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Id of the entity</returns>
        Task<TPrimaryKey> InsertAndGetId(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="entity">Entity</param>
        Task<TEntity> InsertOrUpdate(TEntity entity);

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <param name="entity">Entity</param>
        Task<TEntity> InsertOrUpdate(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// Also returns Id of the entity.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        Task<TPrimaryKey> InsertOrUpdateAndGetId(TEntity entity);

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// Also returns Id of the entity.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Id of the entity</returns>
        Task<TPrimaryKey> InsertOrUpdateAndGetId(TEntity entity, CancellationToken cancellationToken);

        #endregion

        #region Update

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        Task<TEntity> Update(TEntity entity);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <param name="entity">Entity</param>
        Task<TEntity> Update(TEntity entity, CancellationToken cancellationToken);

        #endregion

        #region Delete

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        Task Delete(TEntity entity);

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        /// <param name="cancellationToken">The cancellation token</param>
        Task Delete(TEntity entity, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        Task Delete(TPrimaryKey id);

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        /// <param name="cancellationToken">The cancellation token</param>
        Task Delete(TPrimaryKey id, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        Task Delete(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        /// <param name="cancellationToken">The cancellation token</param>
        Task Delete(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        #endregion

        #region Aggregates

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        Task<int> Count();

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Count of entities</returns>
        Task<int> Count(CancellationToken cancellationToken);

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        Task<int> Count(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Count of entities</returns>
        Task<int> Count(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        Task<long> LongCount();

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Count of entities</returns>
        Task<long> LongCount(CancellationToken cancellationToken);

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        Task<long> LongCount(Expression<Func<TEntity, bool>> predicate);
        
        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>Count of entities</returns>
        Task<long> LongCount(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        #endregion
    }
}
