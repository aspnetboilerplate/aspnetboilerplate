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

        //protected RavenCollection<TEntity> Collection
        //{
        //    get
        //    {
        //        return _databaseProvider.Database.GetCollection<TEntity>(typeof(TEntity).Name);
        //    }
        //}

        protected IQueryable<TEntity> Collection
        {
            get { return _databaseProvider.Database.Query<TEntity>(); }
        }

        public RavenDbRepositoryBase(IRavenDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
        }

        public override IQueryable<TEntity> GetAll()
        {
            //return Collection.AsQueryable();
            //return Database.
            return Collection;
        }

        public override TEntity Get(TPrimaryKey id)
        {
            return _databaseProvider.Database.Load<TEntity>(id.ToString());
        }

        public override TEntity FirstOrDefault(TPrimaryKey id)
        {
            return _databaseProvider.Database.Query<TEntity>().FirstOrDefault(e => e.Id.Equals(id));
            //var query = _databaseProvider.Database.Query<TEntity>(e => e.Id)
            //var query = RavenDB.Driver.Builders.Query<TEntity>.EQ(e => e.Id, id);
            //return Collection.FindOne(query); //TODO: What if no entity with id?
        }

        public override TEntity Insert(TEntity entity)
        {
            //Collection.Insert(entity);
            _databaseProvider.Database.Store(entity);
            return entity;
        }
        public override TEntity Update(TEntity entity)
        {
            //Collection.Save(entity);
            _databaseProvider.Database.Store(entity);
            return entity;
        }

        public override void Delete(TEntity entity)
        {
            Delete(entity.Id);
        }

        public override void Delete(TPrimaryKey id)
        {
            _databaseProvider.Database.Delete(id);
            //var query = RavenDB.Driver.Builders.Query<TEntity>.EQ(e => e.Id, id);
            //Collection.Remove(query);
        }
    }
}