using System.Threading.Tasks;
using Abp.Application.Services;
using MyProject.News.Dtos;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using System.Linq;

namespace MyProject.News
{
    public interface INewsAppService : IApplicationService
    {
        GetNewsOutput GetNews(GetNewsInput input);

        News UpdateNews(UpdateNewsInput input);

        News CreateNews(CreateNewsInput input);


        //   #region Select/Get/Query

        //   IQueryable<News> GetAll();


        IList<News> GetAllList();

        Task<List<News>> GetAllListAsync();

        //   List<News> GetAllList(Expression<Func<News, bool>> predicate) ;


        //   Task<List<News>> GetAllListAsync(Expression<Func<News, bool>> predicate)
        // ;

        //   T Query<T>(Func<IQueryable<News>, T> queryMethod)
        //  ;


        News Get(long id);


        Task<News> GetAsync(long id);


        //   News Single(Expression<Func<News, bool>> predicate)
        //  ;


        //   Task<News> SingleAsync(Expression<Func<News, bool>> predicate)
        // ;


        //   News FirstOrDefault(long id)
        //  ;


        //   Task<News> FirstOrDefaultAsync(long id)
        // ;


        //   News FirstOrDefault(Expression<Func<News, bool>> predicate)
        //  ;


        //   Task<News> FirstOrDefaultAsync(Expression<Func<News, bool>> predicate)
        //  ;

        //   News Load(long id)
        //;


        //   #endregion

        //   #region Insert

        //   /// <summary>
        //   /// Inserts a new entity.
        //   /// </summary>
        //   /// <param name="entity">Inserted entity</param>
        //   News Insert(News entity)
        //  ;

        //   /// <summary>
        //   /// Inserts a new entity.
        //   /// </summary>
        //   /// <param name="entity">Inserted entity</param>
        //   Task<News> InsertAsync(News entity)
        //  ;

        //   /// <summary>
        //   /// Inserts a new entity and gets it's Id.
        //   /// It may require to save current unit of work
        //   /// to be able to retrieve id.
        //   /// </summary>
        //   /// <param name="entity">Entity</param>
        //   /// <returns>Id of the entity</returns>
        //   long InsertAndGetId(News entity)
        //  ;

        //   /// <summary>
        //   /// Inserts a new entity and gets it's Id.
        //   /// It may require to save current unit of work
        //   /// to be able to retrieve id.
        //   /// </summary>
        //   /// <param name="entity">Entity</param>
        //   /// <returns>Id of the entity</returns>
        //   Task<long> InsertAndGetIdAsync(News entity)
        //  ;

        //   /// <summary>
        //   /// Inserts or updates given entity depending on Id's value.
        //   /// </summary>
        //   /// <param name="entity">Entity</param>
        //   News InsertOrUpdate(News entity)
        // ;

        //   /// <summary>
        //   /// Inserts or updates given entity depending on Id's value.
        //   /// </summary>
        //   /// <param name="entity">Entity</param>
        //   Task<News> InsertOrUpdateAsync(News entity)
        // ;

        //   /// <summary>
        //   /// Inserts or updates given entity depending on Id's value.
        //   /// Also returns Id of the entity.
        //   /// It may require to save current unit of work
        //   /// to be able to retrieve id.
        //   /// </summary>
        //   /// <param name="entity">Entity</param>
        //   /// <returns>Id of the entity</returns>
        //   long InsertOrUpdateAndGetId(News entity)
        // ;

        //   /// <summary>
        //   /// Inserts or updates given entity depending on Id's value.
        //   /// Also returns Id of the entity.
        //   /// It may require to save current unit of work
        //   /// to be able to retrieve id.
        //   /// </summary>
        //   /// <param name="entity">Entity</param>
        //   /// <returns>Id of the entity</returns>
        //   Task<long> InsertOrUpdateAndGetIdAsync(News entity)
        // ;

        //   #endregion

        //   #region Update

        //   /// <summary>
        //   /// Updates an existing entity.
        //   /// </summary>
        //   /// <param name="entity">Entity</param>
        //   News Update(News entity)
        //  ;
        //   /// <summary>
        //   /// Updates an existing entity. 
        //   /// </summary>
        //   /// <param name="entity">Entity</param>
        //   Task<News> UpdateAsync(News entity)
        //  ;

        //   /// <summary>
        //   /// Updates an existing entity.
        //   /// </summary>
        //   /// <param name="id">Id of the entity</param>
        //   /// <param name="updateAction">Action that can be used to change values of the entity</param>
        //   /// <returns>Updated entity</returns>
        //   News Update(long id, Action<News> updateAction)
        //  ;

        //   /// <summary>
        //   /// Updates an existing entity.
        //   /// </summary>
        //   /// <param name="id">Id of the entity</param>
        //   /// <param name="updateAction">Action that can be used to change values of the entity</param>
        //   /// <returns>Updated entity</returns>
        //   Task<News> UpdateAsync(long id, Func<News, Task> updateAction)
        // ;

        //   #endregion

        //   #region Delete

        //   /// <summary>
        //   /// Deletes an entity.
        //   /// </summary>
        //   /// <param name="entity">Entity to be deleted</param>
        //   void Delete(News entity)
        // ;

        //   /// <summary>
        //   /// Deletes an entity.
        //   /// </summary>
        //   /// <param name="entity">Entity to be deleted</param>
        //   Task DeleteAsync(News entity)
        //  ;

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        void Delete(long id)
       ;

        //   /// <summary>
        //   /// Deletes an entity by primary key.
        //   /// </summary>
        //   /// <param name="id">Primary key of the entity</param>
        //   Task DeleteAsync(long id)
        //  ;

        //   /// <summary>
        //   /// Deletes many entities by function.
        //   /// Notice that: All entities fits to given predicate are retrieved and deleted.
        //   /// This may cause major performance problems if there are too many entities with
        //   /// given predicate.
        //   /// </summary>
        //   /// <param name="predicate">A condition to filter entities</param>
        //   void Delete(Expression<Func<News, bool>> predicate)
        //  ;

        //   /// <summary>
        //   /// Deletes many entities by function.
        //   /// Notice that: All entities fits to given predicate are retrieved and deleted.
        //   /// This may cause major performance problems if there are too many entities with
        //   /// given predicate.
        //   /// </summary>
        //   /// <param name="predicate">A condition to filter entities</param>
        //   Task DeleteAsync(Expression<Func<News, bool>> predicate)
        //  ;

        //   #endregion

        //   #region Aggregates

        //   /// <summary>
        //   /// Gets count of all entities in this repository.
        //   /// </summary>
        //   /// <returns>Count of entities</returns>
        //   int Count()
        //  ;

        //   /// <summary>
        //   /// Gets count of all entities in this repository.
        //   /// </summary>
        //   /// <returns>Count of entities</returns>
        //   Task<int> CountAsync()
        //  ;

        //   /// <summary>
        //   /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        //   /// </summary>
        //   /// <param name="predicate">A method to filter count</param>
        //   /// <returns>Count of entities</returns>
        //   int Count(Expression<Func<News, bool>> predicate)
        //  ;

        //   /// <summary>
        //   /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        //   /// </summary>
        //   /// <param name="predicate">A method to filter count</param>
        //   /// <returns>Count of entities</returns>
        //   Task<int> CountAsync(Expression<Func<News, bool>> predicate)
        //  ;

        //   /// <summary>
        //   /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        //   /// </summary>
        //   /// <returns>Count of entities</returns>
        //   long LongCount();


        //   /// <summary>
        //   /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        //   /// </summary>
        //   /// <returns>Count of entities</returns>
        //   Task<long> LongCountAsync()
        //  ;

        //   /// <summary>
        //   /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        //   /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        //   /// </summary>
        //   /// <param name="predicate">A method to filter count</param>
        //   /// <returns>Count of entities</returns>
        //   long LongCount(Expression<Func<News, bool>> predicate);


        //   /// <summary>
        //   /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        //   /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        //   /// </summary>
        //   /// <param name="predicate">A method to filter count</param>
        //   /// <returns>Count of entities</returns>
        //   Task<long> LongCountAsync(Expression<Func<News, bool>> predicate)
        //;

        //   #endregion
    }
}
