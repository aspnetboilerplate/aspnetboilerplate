using System.Collections.Generic;
using Abp.Application.Services;
using Abp.Domain.Repositories;
using AutoMapper;
using Reation.CMS.Articles.Dtos;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System;

namespace Reation.CMS.Articles
{
    public class ArticleAppService : ApplicationService, IArticleAppService
    {
        private readonly IRepository<Article, long> _articleRepository;


        public ArticleAppService(IRepository<Article, long> articleRepository)
        {
            _articleRepository = articleRepository;

        }

        public GetArticlesOutput GetArticles(GetArticlesInput input)
        {
            //Called specific GetAllWithPeople method of task repository.
            var news = _articleRepository.GetAllList(x => x.Title.Contains(input.Title));

            //Used AutoMapper to automatically convert List<Task> to List<TaskDto>.
            return new GetArticlesOutput
            {
                News = Mapper.Map<List<ArticleDto>>(news)
            };
        }

        public Article UpdateArticle(UpdateArticleInput input)
        {
            //We can use Logger, it's defined in ApplicationService base class.
            Logger.Info("Updating a article for input: " + input);

            //Retrieving a task entity with given id using standard Get method of repositories.
            var article = _articleRepository.Get(input.Id);

            //Updating changed properties of the retrieved task entity.
            article.Title = input.Title;
            article.Intro = input.Intro;
            article.Content = input.Content;
            return _articleRepository.Update(article);
            //We even do not call Update method of the repository.
            //Because an application service method is a 'unit of work' scope as default.
            //ABP automatically saves all changes when a 'unit of work' scope ends (without any exception).
        }

        public Article CreateArticle(CreateArticleInput input)
        {
            //We can use Logger, it's defined in ApplicationService class.
            Logger.Info("Creating a article for input: " + input);

            //Creating a new Task entity with given input's properties
            var article = new Article
            {
                Title = input.Title,
                Intro = input.Intro,
                Content = input.Content
            };



            //Saving entity with standard Insert method of repositories.
            return _articleRepository.Insert(article);
        }

        //#region Select/Get/Query

        //public IQueryable<News> GetAll()
        //{
        //    return _articleRepository.GetAll();
        //}

        public IList<Article> GetAllList()
        {
            return _articleRepository.GetAllList();
        }

        public Task<List<Article>> GetAllListAsync()
        {
            return _articleRepository.GetAllListAsync();
        }

        //public List<News> GetAllList(Expression<Func<News, bool>> predicate)
        //{
        //    return _articleRepository.GetAllList(predicate);
        //}


        //public Task<List<News>> GetAllListAsync(Expression<Func<News, bool>> predicate)
        //{
        //    return _articleRepository.GetAllListAsync(predicate);
        //}


        //public T Query<T>(Func<IQueryable<News>, T> queryMethod)
        //{
        //    return _articleRepository.Query<T>(queryMethod);
        //}


        public Article Get(long id)
        {
            return _articleRepository.Get(id);
        }


        public Task<Article> GetAsync(long id)
        {
            return _articleRepository.GetAsync(id);
        }


        //public News Single(Expression<Func<News, bool>> predicate)
        //{
        //    return _articleRepository.Single(predicate);
        //}


        //public Task<News> SingleAsync(Expression<Func<News, bool>> predicate)
        //{
        //    return _articleRepository.SingleAsync(predicate);
        //}


        //public News FirstOrDefault(long id)
        //{
        //    return _articleRepository.FirstOrDefault(id);
        //}


        //public Task<News> FirstOrDefaultAsync(long id)
        //{
        //    return _articleRepository.FirstOrDefaultAsync(id);
        //}


        //public News FirstOrDefault(Expression<Func<News, bool>> predicate)
        //{
        //    return _articleRepository.FirstOrDefault(predicate);
        //}


        //public Task<News> FirstOrDefaultAsync(Expression<Func<News, bool>> predicate)
        //{
        //    return _articleRepository.FirstOrDefaultAsync(predicate);
        //}


        //public News Load(long id)
        //{
        //    return _articleRepository.Load(id);
        //}


        //#endregion

        //#region Insert

        ///// <summary>
        ///// Inserts a new entity.
        ///// </summary>
        ///// <param name="entity">Inserted entity</param>
        //public News Insert(News entity)
        //{
        //    return _articleRepository.Insert(entity);
        //}

        ///// <summary>
        ///// Inserts a new entity.
        ///// </summary>
        ///// <param name="entity">Inserted entity</param>
        //public Task<News> InsertAsync(News entity)
        //{
        //    return _articleRepository.InsertAsync(entity);
        //}

        ///// <summary>
        ///// Inserts a new entity and gets it's Id.
        ///// It may require to save current unit of work
        ///// to be able to retrieve id.
        ///// </summary>
        ///// <param name="entity">Entity</param>
        ///// <returns>Id of the entity</returns>
        //public long InsertAndGetId(News entity)
        //{
        //    return _articleRepository.InsertAndGetId(entity);
        //}

        ///// <summary>
        ///// Inserts a new entity and gets it's Id.
        ///// It may require to save current unit of work
        ///// to be able to retrieve id.
        ///// </summary>
        ///// <param name="entity">Entity</param>
        ///// <returns>Id of the entity</returns>
        //public Task<long> InsertAndGetIdAsync(News entity)
        //{
        //    return _articleRepository.InsertAndGetIdAsync(entity);
        //}

        ///// <summary>
        ///// Inserts or updates given entity depending on Id's value.
        ///// </summary>
        ///// <param name="entity">Entity</param>
        //public News InsertOrUpdate(News entity)
        //{
        //    return _articleRepository.InsertOrUpdate(entity);
        //}

        ///// <summary>
        ///// Inserts or updates given entity depending on Id's value.
        ///// </summary>
        ///// <param name="entity">Entity</param>
        //public Task<News> InsertOrUpdateAsync(News entity)
        //{
        //    return _articleRepository.InsertOrUpdateAsync(entity);
        //}

        ///// <summary>
        ///// Inserts or updates given entity depending on Id's value.
        ///// Also returns Id of the entity.
        ///// It may require to save current unit of work
        ///// to be able to retrieve id.
        ///// </summary>
        ///// <param name="entity">Entity</param>
        ///// <returns>Id of the entity</returns>
        //public long InsertOrUpdateAndGetId(News entity)
        //{
        //    return _articleRepository.InsertOrUpdateAndGetId(entity);
        //}

        ///// <summary>
        ///// Inserts or updates given entity depending on Id's value.
        ///// Also returns Id of the entity.
        ///// It may require to save current unit of work
        ///// to be able to retrieve id.
        ///// </summary>
        ///// <param name="entity">Entity</param>
        ///// <returns>Id of the entity</returns>
        //public Task<long> InsertOrUpdateAndGetIdAsync(News entity)
        //{
        //    return _articleRepository.InsertOrUpdateAndGetIdAsync(entity);
        //}

        //#endregion

        //#region Update

        ///// <summary>
        ///// Updates an existing entity.
        ///// </summary>
        ///// <param name="entity">Entity</param>
        //public News Update(News entity)
        //{
        //    return _articleRepository.Update(entity);
        //}

        ///// <summary>
        ///// Updates an existing entity. 
        ///// </summary>
        ///// <param name="entity">Entity</param>
        //public Task<News> UpdateAsync(News entity)
        //{
        //    return _articleRepository.UpdateAsync(entity);
        //}

        ///// <summary>
        ///// Updates an existing entity.
        ///// </summary>
        ///// <param name="id">Id of the entity</param>
        ///// <param name="updateAction">Action that can be used to change values of the entity</param>
        ///// <returns>Updated entity</returns>
        //public News Update(long id, Action<News> updateAction)
        //{
        //    return _articleRepository.Update(id, updateAction);
        //}

        ///// <summary>
        ///// Updates an existing entity.
        ///// </summary>
        ///// <param name="id">Id of the entity</param>
        ///// <param name="updateAction">Action that can be used to change values of the entity</param>
        ///// <returns>Updated entity</returns>
        //public Task<News> UpdateAsync(long id, Func<News, Task> updateAction)
        //{
        //    return _articleRepository.UpdateAsync(id, updateAction);
        //}

        //#endregion

        //#region Delete

        ///// <summary>
        ///// Deletes an entity.
        ///// </summary>
        ///// <param name="entity">Entity to be deleted</param>
        //public void Delete(News entity)
        //{
        //    _articleRepository.Delete(entity);
        //}

        ///// <summary>
        ///// Deletes an entity.
        ///// </summary>
        ///// <param name="entity">Entity to be deleted</param>
        //public Task DeleteAsync(News entity)
        //{
        //    return _articleRepository.DeleteAsync(entity);
        //}

        /// <summary>
        /// Deletes an entity by primary key.
        /// </summary>
        /// <param name="id">Primary key of the entity</param>
        public void Delete(long id)
        {
            _articleRepository.Delete(id);
        }

        ///// <summary>
        ///// Deletes an entity by primary key.
        ///// </summary>
        ///// <param name="id">Primary key of the entity</param>
        //public Task DeleteAsync(long id)
        //{
        //    return _articleRepository.DeleteAsync(id);
        //}

        ///// <summary>
        ///// Deletes many entities by function.
        ///// Notice that: All entities fits to given predicate are retrieved and deleted.
        ///// This may cause major performance problems if there are too many entities with
        ///// given predicate.
        ///// </summary>
        ///// <param name="predicate">A condition to filter entities</param>
        //public void Delete(Expression<Func<News, bool>> predicate)
        //{
        //    _articleRepository.Delete(predicate);
        //}

        ///// <summary>
        ///// Deletes many entities by function.
        ///// Notice that: All entities fits to given predicate are retrieved and deleted.
        ///// This may cause major performance problems if there are too many entities with
        ///// given predicate.
        ///// </summary>
        ///// <param name="predicate">A condition to filter entities</param>
        //public Task DeleteAsync(Expression<Func<News, bool>> predicate)
        //{
        //    return _articleRepository.DeleteAsync(predicate);
        //}

        //#endregion

        //#region Aggregates

        ///// <summary>
        ///// Gets count of all entities in this repository.
        ///// </summary>
        ///// <returns>Count of entities</returns>
        //public int Count()
        //{
        //    return _articleRepository.Count();
        //}

        ///// <summary>
        ///// Gets count of all entities in this repository.
        ///// </summary>
        ///// <returns>Count of entities</returns>
        //public Task<int> CountAsync()
        //{
        //    return _articleRepository.CountAsync();
        //}

        ///// <summary>
        ///// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        ///// </summary>
        ///// <param name="predicate">A method to filter count</param>
        ///// <returns>Count of entities</returns>
        //public int Count(Expression<Func<News, bool>> predicate)
        //{
        //    return _articleRepository.Count(predicate);
        //}

        ///// <summary>
        ///// Gets count of all entities in this repository based on given <paramref name="predicate"/>.
        ///// </summary>
        ///// <param name="predicate">A method to filter count</param>
        ///// <returns>Count of entities</returns>
        //public Task<int> CountAsync(Expression<Func<News, bool>> predicate)
        //{
        //    return _articleRepository.CountAsync(predicate);
        //}

        ///// <summary>
        ///// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        ///// </summary>
        ///// <returns>Count of entities</returns>
        //public long LongCount()
        //{
        //    return _articleRepository.LongCount();
        //}

        ///// <summary>
        ///// Gets count of all entities in this repository (use if expected return value is greather than <see cref="int.MaxValue"/>.
        ///// </summary>
        ///// <returns>Count of entities</returns>
        //public Task<long> LongCountAsync()
        //{
        //    return _articleRepository.LongCountAsync();
        //}

        ///// <summary>
        ///// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        ///// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        ///// </summary>
        ///// <param name="predicate">A method to filter count</param>
        ///// <returns>Count of entities</returns>
        //public long LongCount(Expression<Func<News, bool>> predicate)
        //{
        //    return LongCount(predicate);
        //}

        ///// <summary>
        ///// Gets count of all entities in this repository based on given <paramref name="predicate"/>
        ///// (use this overload if expected return value is greather than <see cref="int.MaxValue"/>).
        ///// </summary>
        ///// <param name="predicate">A method to filter count</param>
        ///// <returns>Count of entities</returns>
        //public Task<long> LongCountAsync(Expression<Func<News, bool>> predicate)
        //{
        //    return _articleRepository.LongCountAsync(predicate);
        //}

        //#endregion
    }
}
