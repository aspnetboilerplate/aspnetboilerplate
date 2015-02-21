using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Domain.Entities;

namespace Abp.Domain.Repositories
{
    /// <summary>
    /// Base class to implement <see cref="IRepository{TEntity,TPrimaryKey}"/>.
    /// It implements some methods in most simple way.
    /// </summary>
    /// <typeparam name="TEntity">Type of the Entity for this repository</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key of the entity</typeparam>
    public abstract class AbpRepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public abstract IQueryable<TEntity> GetAll();
        
        public virtual List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        public abstract Task<List<TEntity>> GetAllListAsync();
        
        public virtual List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public abstract Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);

        public virtual T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return queryMethod(GetAll());
        }
        
        public virtual TEntity Get(TPrimaryKey id)
        {
            var entity = FirstOrDefault(id);
            if (entity == null)
            {
                throw new AbpException("There is no such an entity with given primary key. Entity type: " + typeof(TEntity).FullName + ", primary key: " + id);
            }

            return entity;
        }

        public virtual async Task<TEntity> GetAsync(TPrimaryKey id)
        {
            var entity = await FirstOrDefaultAsync(id);
            if (entity == null)
            {
                throw new AbpException("There is no such an entity with given primary key. Entity type: " + typeof(TEntity).FullName + ", primary key: " + id);
            }

            return entity;
        }
        
        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Single(predicate);
        }

        public abstract Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);
        
        public virtual TEntity FirstOrDefault(TPrimaryKey id)
        {
            return GetAll().FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        public abstract Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id);
        
        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public abstract Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        
        public virtual TEntity Load(TPrimaryKey id)
        {
            return Get(id);
        }

        public abstract TEntity Insert(TEntity entity);
        public abstract Task<TEntity> InsertAsync(TEntity entity);
        public abstract TPrimaryKey InsertAndGetId(TEntity entity);
        public abstract Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity);

        public virtual TEntity InsertOrUpdate(TEntity entity)
        {
            return EqualityComparer<TPrimaryKey>.Default.Equals(entity.Id, default(TPrimaryKey))
                ? Insert(entity)
                : Update(entity);
        }
        
        public virtual async Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            return EqualityComparer<TPrimaryKey>.Default.Equals(entity.Id, default(TPrimaryKey))
                ? await InsertAsync(entity)
                : await UpdateAsync(entity);
        }

        public abstract TPrimaryKey InsertOrUpdateAndGetId(TEntity entity);
        public abstract Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity);
        public abstract TEntity Update(TEntity entity);
        public abstract Task<TEntity> UpdateAsync(TEntity entity);
        public abstract void Delete(TEntity entity);
        
        public virtual async Task DeleteAsync(TEntity entity)
        {
            Delete(entity);
        }

        public abstract void Delete(TPrimaryKey id);
        
        public virtual async Task DeleteAsync(TPrimaryKey id)
        {
            Delete(id);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in GetAll().Where(predicate).ToList())
            {
                Delete(entity);
            }
        }

        public virtual async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Delete(predicate);
        }

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

        protected static Expression<Func<TEntity, bool>> CreateEqualityExpressionForId(TPrimaryKey id)
        {
            var lambdaParam = Expression.Parameter(typeof(TEntity));

            var lambdaBody = Expression.Equal(
                Expression.PropertyOrField(lambdaParam, "Id"),
                Expression.Constant(id, typeof(TPrimaryKey))
                );

            return Expression.Lambda<Func<TEntity, bool>>(lambdaBody, lambdaParam);
        }
    }
}