using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Domain.Uow.EntityFramework;

namespace Abp.Domain.Repositories.EntityFramework
{
    public abstract class EfRepositoryBase<TDbContext, TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
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

        public virtual TEntity Update(TEntity entity)
        {
            Table.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public virtual void Delete(TEntity entity)
        {
            Table.Remove(entity);
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

                Table.Attach(entity); //TODO: Is this needed? Test!
            }

            Table.Remove(entity);

            /* TODO: Enable removing using SQL. Should use table and primary key name!
            return Context.Database.ExecuteSqlCommand(
                "DELETE FROM " + TableName + " WHERE Id = @Id",
                new SqlParameter("@Id", key)
                );
             */
        }
        public virtual void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entities = Table.Where(predicate).ToList();
            foreach (var entity in entities)
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
