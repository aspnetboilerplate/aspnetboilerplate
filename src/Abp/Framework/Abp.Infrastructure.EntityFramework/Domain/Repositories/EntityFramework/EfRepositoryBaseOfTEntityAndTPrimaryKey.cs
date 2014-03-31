using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Exceptions;

namespace Abp.Domain.Repositories.EntityFramework
{
    /// <summary>
    /// Base class for all repositories those uses Entity Framework.
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key type of the entity</typeparam>
    public class EfRepositoryBase<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>, new()
    {
        protected AbpDbContext Context { get { return ((EfUnitOfWork)UnitOfWorkScope.CurrentUow).Context; } }

        protected DbSet<TEntity> Table { get { return Context.Set<TEntity>(); } }

        public virtual IQueryable<TEntity> GetAll()
        {
            IQueryable<TEntity> query = Table;

            if (typeof(ISoftDeleteEntity).IsAssignableFrom(typeof(TEntity)))
            {
                query = query.Where(entity => !(entity as ISoftDeleteEntity).IsDeleted);
            }

            return query;
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
            return GetAll().Where("Id = @0", key).FirstOrDefault(); //TODO: Implement ISoftDeleteEntity?
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public virtual TEntity Load(TPrimaryKey key)
        {
            //TODO: 
            return Get(key); //TODO: Implement ISoftDeleteEntity?
        }

        public virtual TEntity Insert(TEntity entity)
        {
            Table.Add(entity);
            return entity;
        }

        public virtual TEntity Update(TEntity entity)
        {
            Table.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual void Delete(TEntity entity)
        {

            if (entity is ISoftDeleteEntity)
            {
                (entity as ISoftDeleteEntity).IsDeleted = true;
                Update(entity);
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
                entity = new TEntity { Id = id };
                Table.Attach(entity);
            }

            Table.Remove(entity);

            /* TODO: Enable removing using SQL. Get TableName!             
            return Context.Database.ExecuteSqlCommand(
                "DELETE FROM " + TableName + " WHERE Id = @Id",
                new SqlParameter("@Id", key)
                );
             */
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
