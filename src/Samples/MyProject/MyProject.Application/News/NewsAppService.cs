using System.Collections.Generic;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using AutoMapper;
using MyProject.News.Dtos;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;

namespace MyProject.News
{
    public class NewsAppService : ApplicationService, INewsAppService
    {
        private readonly IRepository<News,long> _newsRepository;
      

        public NewsAppService(IRepository<News,long> newsRepository)
        {
            _newsRepository = newsRepository;
            
        }

        //private readonly INewsRepository _newsRepository;
        //public NewsAppService(INewsRepository newsRepository)
        //{
        //    _newsRepository = newsRepository;
        //}

        #region Select/Get/Query

        public IQueryable<News> GetAll()
        {
            return _newsRepository.GetAll();
        }

        public IList<News> GetAllList()
        {
            return _newsRepository.GetAllList();
        }

        public Task<List<News>> GetAllListAsync()
        {
            return _newsRepository.GetAllListAsync();
        }

        public List<News> GetAllList(Expression<Func<News, bool>> predicate)
        {
            return _newsRepository.GetAllList(predicate);
        }


        public Task<List<News>> GetAllListAsync(Expression<Func<News, bool>> predicate)
        {
            return _newsRepository.GetAllListAsync(predicate);
        }


        public T Query<T>(Func<IQueryable<News>, T> queryMethod)
        {
            return _newsRepository.Query<T>(queryMethod);
        }


        public News Get(long id)
        {
            return _newsRepository.Get(id);
        }


        public Task<News> GetAsync(long id)
        {
            return _newsRepository.GetAsync(id);
        }


        public News Single(Expression<Func<News, bool>> predicate)
        {
            return _newsRepository.Single(predicate);
        }


        public Task<News> SingleAsync(Expression<Func<News, bool>> predicate)
        {
            return _newsRepository.SingleAsync(predicate);
        }


        public News FirstOrDefault(long id)
        {
            return _newsRepository.FirstOrDefault(id);
        }


        public Task<News> FirstOrDefaultAsync(long id)
        {
            return _newsRepository.FirstOrDefaultAsync(id);
        }


        public News FirstOrDefault(Expression<Func<News, bool>> predicate)
        {
            return _newsRepository.FirstOrDefault(predicate);
        }


        public Task<News> FirstOrDefaultAsync(Expression<Func<News, bool>> predicate)
        {
            return _newsRepository.FirstOrDefaultAsync(predicate);
        }


        public News Load(long id)
        {
            return _newsRepository.Load(id);
        }


        #endregion

        #region Insert

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        public News Insert(News entity)
        {
            return _newsRepository.Insert(entity);
        }

        /// <summary>
        /// Inserts a new entity.
        /// </summary>
        /// <param name="entity">Inserted entity</param>
        public Task<News> InsertAsync(News entity)
        {
            return _newsRepository.InsertAsync(entity);
        }

        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        public long InsertAndGetId(News entity)
        {
            return _newsRepository.InsertAndGetId(entity);
        }

        /// <summary>
        /// Inserts a new entity and gets it's Id.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        public Task<long> InsertAndGetIdAsync(News entity)
        {
            return _newsRepository.InsertAndGetIdAsync(entity);
        }

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="entity">Entity</param>
        public News InsertOrUpdate(News entity)
        {
            return _newsRepository.InsertOrUpdate(entity);
        }

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// </summary>
        /// <param name="entity">Entity</param>
        public Task<News> InsertOrUpdateAsync(News entity)
        {
            return _newsRepository.InsertOrUpdateAsync(entity);
        }

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// Also returns Id of the entity.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        public long InsertOrUpdateAndGetId(News entity)
        {
            return _newsRepository.InsertOrUpdateAndGetId(entity);
        }

        /// <summary>
        /// Inserts or updates given entity depending on Id's value.
        /// Also returns Id of the entity.
        /// It may require to save current unit of work
        /// to be able to retrieve id.
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <returns>Id of the entity</returns>
        public Task<long> InsertOrUpdateAndGetIdAsync(News entity)
        {
            return _newsRepository.InsertOrUpdateAndGetIdAsync(entity);
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="entity">Entity</param>
        public News Update(News entity)
        {
            return _newsRepository.Update(entity);
        }

        /// <summary>
        /// Updates an existing entity. 
        /// </summary>
        /// <param name="entity">Entity</param>
        public Task<News> UpdateAsync(News entity)
        {
            return _newsRepository.UpdateAsync(entity);
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="updateAction">Action that can be used to change values of the entity</param>
        /// <returns>Updated entity</returns>
        public News Update(long id, Action<News> updateAction)
        {
            return _newsRepository.Update(id, updateAction);
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <param name="id">Id of the entity</param>
        /// <param name="updateAction">Action that can be used to change values of the entity</param>
        /// <returns>Updated entity</returns>
        public Task<News> UpdateAsync(long id, Func<News, Task> updateAction)
        {
            return _newsRepository.UpdateAsync(id, updateAction);
        }

        #endregion

        #region Delete

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public void Delete(News entity)
        {
            _newsRepository.Delete(entity);
        }

        /// <summary>
        /// Deletes an entity.
        /// </summary>
        /// <param name="entity">Entity to be deleted</param>
        public Task DeleteAsync(News entity)
        {
            return _newsRepository.DeleteAsync(entity);
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        public void Delete(long id)
        {
            _newsRepository.Delete(id);
        }

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        public Task DeleteAsync(long id)
        {
            return _newsRepository.DeleteAsync(id);
        }

        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        public void Delete(Expression<Func<News, bool>> predicate)
        {
            _newsRepository.Delete(predicate);
        }

        /// <summary>
        /// Deletes many entities by function.
        /// Notice that: All entities fits to given predicate are retrieved and deleted.
        /// This may cause major performance problems if there are too many entities with
        /// given predicate.
        /// </summary>
        /// <param name="predicate">A condition to filter entities</param>
        public Task DeleteAsync(Expression<Func<News, bool>> predicate)
        {
            return _newsRepository.DeleteAsync(predicate);
        }

        #endregion

        #region Aggregates

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        public int Count()
        {
            return _newsRepository.Count();
        }

        /// <summary>
        /// Gets count of all entities in this repository.
        /// </summary>
        /// <returns>Count of entities</returns>
        public Task<int> CountAsync()
        {
            return _newsRepository.CountAsync();
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public int Count(Expression<Func<News, bool>> predicate)
        {
            return _newsRepository.Count(predicate);
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public Task<int> CountAsync(Expression<Func<News, bool>> predicate)
        {
            return _newsRepository.CountAsync(predicate);
        }

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        public long LongCount()
        {
            return _newsRepository.LongCount();
        }

        /// <summary>
        /// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        /// </summary>
        /// <returns>Count of entities</returns>
        public Task<long> LongCountAsync()
        {
            return _newsRepository.LongCountAsync();
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public long LongCount(Expression<Func<News, bool>> predicate)
        {
            return LongCount(predicate);
        }

        /// <summary>
        /// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        /// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        /// </summary>
        /// <param name="predicate">A method to filter count</param>
        /// <returns>Count of entities</returns>
        public Task<long> LongCountAsync(Expression<Func<News, bool>> predicate)
        {
            return _newsRepository.LongCountAsync(predicate);
        }

        #endregion
    }
}
