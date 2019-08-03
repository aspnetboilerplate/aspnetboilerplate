using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using NHibernate;
using NHibernate.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Abp.NHibernate.Repositories
{
    /// <summary>
    /// Base class for all repositories those uses NHibernate.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public class NhRepositoryBase<TEntity, TPrimaryKey> : AbpRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        /// <summary>
        /// Gets the NHibernate session object to perform database operations.
        /// </summary>
        public virtual ISession Session { get { return _sessionProvider.Session; } }

        private readonly ISessionProvider _sessionProvider;

        /// <summary>
        /// Creates a new <see cref="NhRepositoryBase{TEntity,TPrimaryKey}"/> object.
        /// </summary>
        /// <param name="sessionProvider">A session provider to obtain session for database operations</param>
        public NhRepositoryBase(ISessionProvider sessionProvider)
        {
            _sessionProvider = sessionProvider;
        }

        public override IQueryable<TEntity> GetAll()
        {
            return Session.Query<TEntity>();
        }

        public override IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            if (propertySelectors.IsNullOrEmpty())
            {
                return GetAll();
            }

            var query = GetAll();

            foreach (var propertySelector in propertySelectors)
            {
                //TODO: Test if NHibernate supports multiple fetch.
                query = query.Fetch(propertySelector);
            }

            return query;
        }

        public override Task<List<TEntity>> GetAllListAsync()
        {
            return GetAll().ToListAsync();
        }

        public override Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToListAsync();
        }

        public override Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().SingleAsync(predicate);
        }

        public override TEntity FirstOrDefault(TPrimaryKey id)
        {
            return Session.Get<TEntity>(id);
        }

        public override Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id)
        {
            return Session.GetAsync<TEntity>(id);
        }

        public override async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().FirstOrDefaultAsync(predicate);
        }

        public override TEntity Load(TPrimaryKey id)
        {
            return Session.Load<TEntity>(id);
        }

        public override TEntity Insert(TEntity entity)
        {
            Session.Save(entity);
            return entity;
        }

        public override async Task<TEntity> InsertAsync(TEntity entity)
        {
            await Session.SaveAsync(entity);
            return entity;
        }

        public override TEntity InsertOrUpdate(TEntity entity)
        {
            Session.SaveOrUpdate(entity);
            return entity;
        }

        public override async Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            await Session.SaveOrUpdateAsync(entity);
            return entity;
        }

        public override TEntity Update(TEntity entity)
        {
            Session.Update(entity);
            return entity;
        }

        public override async Task<TEntity> UpdateAsync(TEntity entity)
        {
            await Session.UpdateAsync(entity);
            return entity;
        }

        public override void Delete(TEntity entity)
        {
            if (entity is ISoftDelete softDeleteEntity)
            {
                softDeleteEntity.IsDeleted = true;
                Update(entity);
            }
            else
            {
                Session.Delete(entity);
            }
        }

        public override void Delete(TPrimaryKey id)
        {
            var entity = Load(id);

            Delete(entity);
        }

        public override async Task DeleteAsync(TEntity entity)
        {
            if (entity is ISoftDelete softDeleteEntity)
            {
                softDeleteEntity.IsDeleted = true;
                await UpdateAsync(entity);
            }
            else
            {
                await Session.DeleteAsync(entity);
            }
        }

        public override async Task DeleteAsync(TPrimaryKey id)
        {
            var entity = Load(id);

            await DeleteAsync(entity);
        }

        public override async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = await GetAllListAsync(predicate);

            foreach (var entity in entities)
            {
                await DeleteAsync(entity);
            }
        }

        public override Task<int> CountAsync()
        {
            return GetAll().CountAsync();
        }

        public override Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().CountAsync(predicate);
        }

        public override Task<long> LongCountAsync()
        {
            return GetAll().LongCountAsync();
        }

        public override Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().LongCountAsync(predicate);
        }
    }
}