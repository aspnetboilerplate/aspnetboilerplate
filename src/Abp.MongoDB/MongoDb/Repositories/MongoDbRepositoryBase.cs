using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Abp.MongoDb.Repositories
{
    /// <summary>
    /// Implements IRepository for MongoDB.
    /// </summary>
    /// <typeparam name="TEntity">Type of the Entity for this repository</typeparam>
    public class MongoDbRepositoryBase<TEntity> : MongoDbRepositoryBase<TEntity, int>, IRepository<TEntity>
        where TEntity : class, IEntity<int>
    {
        public MongoDbRepositoryBase(IMongoDatabaseProvider databaseProvider)
            : base(databaseProvider)
        {
        }
    }

    /// <summary>
    /// Implements IRepository for MongoDB.
    /// </summary>
    /// <typeparam name="TEntity">Type of the Entity for this repository</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key of the entity</typeparam>
    public class MongoDbRepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        private readonly IMongoDatabaseProvider _databaseProvider;

        protected MongoDatabase Database
        {
            get { return _databaseProvider.Database; }
        }

        protected MongoCollection<TEntity> Collection
        {
            get
            {
                return _databaseProvider.Database.GetCollection<TEntity>(typeof(TEntity).Name);
            }
        }

        public MongoDbRepositoryBase(IMongoDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
        }

        public IQueryable<TEntity> GetAll()
        {
            return Collection.AsQueryable();
        }

        public List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        public Task<List<TEntity>> GetAllListAsync()
        {
            return Task.FromResult(GetAllList());
        }

        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(GetAllList(predicate));
        }

        public T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return queryMethod(GetAll());
        }

        public TEntity Get(TPrimaryKey id)
        {
            var query = MongoDB.Driver.Builders.Query<TEntity>.EQ(e => e.Id, id);
            return Collection.FindOne(query); //TODO: What if no entity with id?
        }

        public Task<TEntity> GetAsync(TPrimaryKey id)
        {
            return Task.FromResult(Get(id));
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Single(predicate);
        }

        public Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Single(predicate));
        }

        public TEntity FirstOrDefault(TPrimaryKey id)
        {
            var query = MongoDB.Driver.Builders.Query<TEntity>.EQ(e => e.Id, id);
            return Collection.FindOne(query); //TODO: What if no entity with id?
        }

        public Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id)
        {
            return Task.FromResult(FirstOrDefault(id));
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(FirstOrDefault(predicate));
        }

        public TEntity Load(TPrimaryKey id)
        {
            return Get(id);
        }

        public TEntity Insert(TEntity entity)
        {
            Collection.Insert(entity);
            return entity;
        }

        public Task<TEntity> InsertAsync(TEntity entity)
        {
            Collection.Insert(entity);
            return Task.FromResult(entity);
        }

        public TPrimaryKey InsertAndGetId(TEntity entity)
        {
            Collection.Insert(entity);
            return entity.Id;
        }

        public Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            return Task.FromResult(InsertAndGetId(entity));
        }

        public TEntity InsertOrUpdate(TEntity entity)
        {
            return EqualityComparer<TPrimaryKey>.Default.Equals(entity.Id, default(TPrimaryKey))
                ? Insert(entity)
                : Update(entity);
        }

        public Task<TEntity> InsertOrUpdateAsync(TEntity entity)
        {
            return Task.FromResult(InsertOrUpdate(entity));
        }

        public TPrimaryKey InsertOrUpdateAndGetId(TEntity entity)
        {
            entity = InsertOrUpdate(entity);
            return entity.Id;
        }

        public Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
        {
            return Task.FromResult(InsertOrUpdateAndGetId(entity));
        }

        public TEntity Update(TEntity entity)
        {
            Collection.Save(entity);
            return entity;
        }

        public Task<TEntity> UpdateAsync(TEntity entity)
        {
            return Task.FromResult(Update(entity));
        }

        public void Delete(TEntity entity)
        {
            Delete(entity.Id);
        }

        public async Task DeleteAsync(TEntity entity)
        {
            Delete(entity);
        }

        public void Delete(TPrimaryKey id)
        {
            var query = MongoDB.Driver.Builders.Query<TEntity>.EQ(e => e.Id, id);
            Collection.Remove(query);
        }

        public async Task DeleteAsync(TPrimaryKey id)
        {
            Delete(id);
        }

        public void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in GetAll().Where(predicate).ToList())
            {
                Delete(entity);
            }
        }

        public async Task DeleteAsync(Expression<Func<TEntity, bool>> predicate)
        {
            Delete(predicate);
        }

        public int Count()
        {
            return GetAll().Count();
        }

        public Task<int> CountAsync()
        {
            return Task.FromResult(Count());
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Count(predicate);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(Count(predicate));
        }

        public long LongCount()
        {
            return GetAll().LongCount();
        }

        public Task<long> LongCountAsync()
        {
            return Task.FromResult(LongCount());
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().LongCount(predicate);
        }

        public Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Task.FromResult(LongCount(predicate));
        }
    }
}