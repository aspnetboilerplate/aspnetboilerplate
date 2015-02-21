using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Abp.Domain.Repositories
{
    public abstract class AbpRepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public abstract IQueryable<TEntity> GetAll();
        public abstract List<TEntity> GetAllList();
        public abstract Task<List<TEntity>> GetAllListAsync();
        public abstract List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);
        public abstract Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);
        public abstract T Query<T>(Func<IQueryable<TEntity>, T> queryMethod);
        public abstract TEntity Get(TPrimaryKey id);
        public abstract Task<TEntity> GetAsync(TPrimaryKey id);
        public abstract TEntity Single(Expression<Func<TEntity, bool>> predicate);
        public abstract Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);
        public abstract TEntity FirstOrDefault(TPrimaryKey id);
        public abstract Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id);
        public abstract TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);
        public abstract Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        public abstract TEntity Load(TPrimaryKey id);
        public abstract TEntity Insert(TEntity entity);
        public abstract Task<TEntity> InsertAsync(TEntity entity);
        public abstract TPrimaryKey InsertAndGetId(TEntity entity);
        public abstract Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity);
        public abstract TEntity InsertOrUpdate(TEntity entity);
        public abstract Task<TEntity> InsertOrUpdateAsync(TEntity entity);
        public abstract TPrimaryKey InsertOrUpdateAndGetId(TEntity entity);
        public abstract Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity);
        public abstract TEntity Update(TEntity entity);
        public abstract Task<TEntity> UpdateAsync(TEntity entity);
        public abstract void Delete(TEntity entity);
        public abstract Task DeleteAsync(TEntity entity);
        public abstract void Delete(TPrimaryKey id);
        public abstract Task DeleteAsync(TPrimaryKey id);

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in GetAll().Where(predicate).ToList())
            {
                Delete(entity);
            }
        }
        
        public abstract Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        public virtual int Count()
        {
            return GetAll().Count();
        }

        public abstract Task<int> CountAsync();

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).Count();
        }

        public abstract Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        public virtual long LongCount()
        {
            return GetAll().LongCount();
        }

        public abstract Task<long> LongCountAsync();

        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).LongCount();
        }

        public abstract Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}