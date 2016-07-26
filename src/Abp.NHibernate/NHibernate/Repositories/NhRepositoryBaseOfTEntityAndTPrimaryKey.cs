using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Collections.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using NHibernate;
using NHibernate.Linq;
using NHibernate.Util;

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

        public override TEntity FirstOrDefault(TPrimaryKey id)
        {
            return Session.Get<TEntity>(id);
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

        public override TEntity InsertOrUpdate(TEntity entity)
        {
            Session.SaveOrUpdate(entity);
            return entity;
        }

        public override Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            return Task.FromResult(InsertOrUpdate(entity));
        }

        public override TEntity Update(TEntity entity)
        {
            Session.Update(entity);
            return entity;
        }

        public override void Delete(TEntity entity)
        {
            if (entity is ISoftDelete)
            {
                (entity as ISoftDelete).IsDeleted = true;
                Update(entity);
            }
            else
            {
                Session.Delete(entity);
            }
        }

        public override void Delete(TPrimaryKey id)
        {
            Delete(Session.Load<TEntity>(id));
        }
    }
}