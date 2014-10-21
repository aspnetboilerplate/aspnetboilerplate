using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.EntityFramework.Uow;

namespace Abp.EntityFramework.Repositories
{
    public class EfRepositoryBase<TDbContext, TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TDbContext : DbContext
    {
        protected virtual TDbContext Context { get { return UnitOfWorkScope.Current.GetDbContext<TDbContext>(); } }

        protected virtual DbSet<TEntity> Table { get { return Context.Set<TEntity>(); } }

        public virtual IQueryable<TEntity> GetAll()
        {
            return Table;
        }

        public virtual List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        public virtual List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public virtual T Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return queryMethod(GetAll());
        }

        public virtual TEntity Get(TPrimaryKey key)
        {
            var entity = FirstOrDefault(key);
            if (entity == null)
            {
                throw new AbpException("Threre is no such an entity with given primary key. Entity type: " + typeof(TEntity).FullName + ", primary key: " + key);
            }

            return entity;
        }

        public virtual TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Single(predicate);
        }

        public virtual TEntity FirstOrDefault(TPrimaryKey key)
        {
            return GetAll().Where("Id = @0", key).FirstOrDefault(); //TODO: Get actual primary key name
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public virtual TEntity Load(TPrimaryKey key)
        {
            return Get(key); //EntityFramework has no Load as like NHibernate.
        }

        public virtual TEntity Insert(TEntity entity)
        {
            return Table.Add(entity);
        }

        public TPrimaryKey InsertAndGetId(TEntity entity)
        {
            entity = Insert(entity);
            
            if (EqualityComparer<TPrimaryKey>.Default.Equals(entity.Id, default(TPrimaryKey)))
            {
                UnitOfWorkScope.Current.SaveChanges();
            }

            return entity.Id;
        }

        public TEntity InsertOrUpdate(TEntity entity)
        {
            return EqualityComparer<TPrimaryKey>.Default.Equals(entity.Id, default(TPrimaryKey))
                ? Insert(entity)
                : Update(entity);
        }

        public TPrimaryKey InsertOrUpdateAndGetId(TEntity entity)
        {
            entity = InsertOrUpdate(entity);

            if (EqualityComparer<TPrimaryKey>.Default.Equals(entity.Id, default(TPrimaryKey)))
            {
                UnitOfWorkScope.Current.SaveChanges();
            }

            return entity.Id;
        }

        public virtual TEntity Update(TEntity entity)
        {
            Table.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual void Delete(TEntity entity)
        {
            if (entity is ISoftDelete)
            {
                (entity as ISoftDelete).IsDeleted = true;
            }
            else
            {
                Table.Remove(entity);                
            }
        }

        public virtual void Delete(TPrimaryKey id)
        {
            var entity = Table.Local.FirstOrDefault(ent => EqualityComparer<TPrimaryKey>.Default.Equals(ent.Id, id));
            if (entity == null)
            {
                entity = FirstOrDefault(id);
                if (entity == null)
                {
                    return;
                }
            }

            Delete(entity);
        }

        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            foreach (var entity in Table.Where(predicate).ToList())
            {
                Delete(entity);
            }
        }
        
        public virtual int Count()
        {
            return GetAll().Count();
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).Count();
        }

        public virtual long LongCount()
        {
            return GetAll().LongCount();
        }

        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).LongCount();
        }
    }
}
