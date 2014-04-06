using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Utils.Extensions.Collections;

namespace Abp.Modules.Core.Tests.Settings
{
    public class MemoryBasedRepository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        private readonly IPrimaryKeyGenerator<TPrimaryKey> _primaryKeyGenerator;

        private readonly Dictionary<TPrimaryKey, TEntity> _entities;

        public MemoryBasedRepository(IPrimaryKeyGenerator<TPrimaryKey> primaryKeyGenerator)
        {
            _primaryKeyGenerator = primaryKeyGenerator;
            _entities = new Dictionary<TPrimaryKey, TEntity>();
        }

        public IQueryable<TEntity> GetAll()
        {
            return _entities.Values.AsQueryable();
        }

        public List<TEntity> GetAllList()
        {
            return _entities.Values.ToList();
        }

        public List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return queryMethod(GetAll());
        }

        public TEntity Get(TPrimaryKey key)
        {
            var entity = FirstOrDefault(key);
            if (entity == null)
            {
                throw new AbpException("Threre is no such an entity with given primary key. Entity type: " + typeof(TEntity).FullName + ", primary key: " + key);
            }

            return entity;
        }

        public TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Single(predicate);
        }

        public TEntity FirstOrDefault(TPrimaryKey key)
        {
            return _entities.GetOrDefault(key);
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public TEntity Load(TPrimaryKey key)
        {
            return Get(key);
        }

        public TEntity Insert(TEntity entity)
        {
            entity.Id = _primaryKeyGenerator.Generate();
            _entities[entity.Id] = entity;
            return entity;
        }

        public TEntity Update(TEntity entity)
        {
            _entities[entity.Id] = entity;
            return entity;
        }

        public void Delete(TEntity entity)
        {
            _entities.Remove(entity.Id);
        }

        public void Delete(TPrimaryKey id)
        {
            _entities.Remove(id);
        }

        public int Count()
        {
            return GetAll().Count();
        }

        public int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Count(predicate);
        }

        public long LongCount()
        {
            return GetAll().LongCount();
        }

        public long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().LongCount(predicate);
        }
    }
}