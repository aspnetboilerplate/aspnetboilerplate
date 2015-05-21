using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Linq;

namespace Abp.RavenDb.Repositories
{
    /// <summary>
    /// Implements IRepository for RavenDB.
    /// </summary>
    /// <typeparam name="TEntity">Type of the Entity for this repository</typeparam>
    public class RavenDbRepositoryBase<TEntity> : RavenDbRepositoryBase<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        public RavenDbRepositoryBase(IRavenDatabaseProvider databaseProvider)
            : base(databaseProvider)
        {
        }
    }

    /// <summary>
    /// Implements IRepository for RavenDB.
    /// </summary>
    /// <typeparam name="TEntity">Type of the Entity for this repository</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key of the entity</typeparam>
    public class RavenDbRepositoryBase<TEntity, TPrimaryKey> : AbpRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        private readonly IRavenDatabaseProvider _databaseProvider;

        protected IDocumentSession Database
        {
            get { return _databaseProvider.Database; }
        }

        protected IQueryable<TEntity> Collection
        {
            get { return Database.Query<TEntity>(); }
        }

        public RavenDbRepositoryBase(IRavenDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
        }

        public override IQueryable<TEntity> GetAll()
        {
            return Collection;
        }

        public override TEntity Get(TPrimaryKey id)
        {
            return Database.Load<TEntity>(id.ToString());
        }

        public override TEntity FirstOrDefault(TPrimaryKey id)
        {
            return Database.Query<TEntity>().FirstOrDefault(e => e.Id.Equals(id));
        }

        public override TEntity Insert(TEntity entity)
        {
            _databaseProvider.Database.Store(entity);
            return entity;
        }

        public override TEntity Update(TEntity entity)
        {
            return Insert(entity);
        }

        public override void Delete(TEntity entity)
        {
            Delete(entity.Id);
        }

        public override void Delete(TPrimaryKey id)
        {
            _databaseProvider.Database.Delete(id);
        }
    }
}