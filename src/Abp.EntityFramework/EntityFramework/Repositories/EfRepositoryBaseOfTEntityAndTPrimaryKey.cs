using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;

namespace Abp.EntityFramework.Repositories
{
    public class EfRepositoryBase<TDbContext, TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TDbContext : DbContext
    {
        public virtual IDbContextProvider<TDbContext> DbContextProvider { private get; set; }

        protected virtual TDbContext Context { get { return DbContextProvider.GetDbContext(); } }

        protected virtual DbSet<TEntity> Table { get { return Context.Set<TEntity>(); } }
        
        public EfRepositoryBase()
        {
            DbContextProvider = DefaultContextProvider<TDbContext>.Instance;
        }

        public EfRepositoryBase(Func<TDbContext> dbContextFactory)
        {
            DbContextProvider = new FactoryContextProvider<TDbContext>(dbContextFactory);
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return Table;
        }

        public virtual List<TEntity> GetAllList()
        {
            return GetAll().ToList();
        }

        public virtual async Task<List<TEntity>> GetAllListAsync()
        {
            return await GetAll().ToListAsync();
        }

        public virtual List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }
        public virtual async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).ToListAsync();
        }

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

        public virtual async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().SingleAsync(predicate);
        }

        public virtual TEntity FirstOrDefault(TPrimaryKey id)
        {
            return GetAll()
                .FirstOrDefault(CreateEqualityExpression(id));
        }

        public virtual async Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id)
        {
            return await GetAll()
                .FirstOrDefaultAsync(CreateEqualityExpression(id));
        }
        
        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().FirstOrDefault(predicate);
        }

        public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().FirstOrDefaultAsync(predicate);
        }

        public virtual TEntity Load(TPrimaryKey id)
        {
            return Get(id); //EntityFramework has no Load as like NHibernate.
        }

        public virtual TEntity Insert(TEntity entity)
        {
            return Table.Add(entity);
        }

        public virtual TPrimaryKey InsertAndGetId(TEntity entity)
        {
            entity = Insert(entity);
            
            if (EqualityComparer<TPrimaryKey>.Default.Equals(entity.Id, default(TPrimaryKey)))
            {
                UnitOfWorkScope.Current.SaveChanges();
            }

            return entity.Id;
        }
        public virtual async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            entity = Insert(entity);

            if (EqualityComparer<TPrimaryKey>.Default.Equals(entity.Id, default(TPrimaryKey)))
            {
                UnitOfWorkScope.Current.SaveChanges(); //TODO: Call Async
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

        public virtual async Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
        {
            entity = InsertOrUpdate(entity);

            if (EqualityComparer<TPrimaryKey>.Default.Equals(entity.Id, default(TPrimaryKey)))
            {
                UnitOfWorkScope.Current.SaveChanges(); //TODO: Call Async
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

        public virtual async Task<int> CountAsync()
        {
            return await GetAll().CountAsync();
        }

        public virtual int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).Count();
        }

        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).CountAsync();
        }

        public virtual long LongCount()
        {
            return GetAll().LongCount();
        }

        public virtual async Task<long> LongCountAsync()
        {
            return await GetAll().LongCountAsync();
        }

        public virtual long LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).LongCount();
        }

        public virtual async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).LongCountAsync();
        }

        private static Expression<Func<TEntity, bool>> CreateEqualityExpression(TPrimaryKey id)
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
