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
       private readonly INewsRepository _newsRepository;
        public NewsAppService(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }
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


        News Get(long id)
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
            return _newsRepository.FirstOrDefault(id );
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
    }
}
