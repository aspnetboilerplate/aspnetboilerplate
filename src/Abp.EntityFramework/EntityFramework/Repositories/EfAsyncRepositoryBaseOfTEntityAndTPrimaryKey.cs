using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Abp.EntityFramework.Repositories
{
    /// <inheritdoc />
    public class EfAsyncRepositoryBase<TDbContext, TEntity, TPrimaryKey> : IAsyncRepository<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TDbContext : DbContext
    {
        /// <inheritdoc />
        public virtual IDbContextProvider<TDbContext> DbContextProvider { private get; set; }

        /// <inheritdoc />
        protected virtual TDbContext Context { get { return DbContextProvider.GetDbContext(); } }

        /// <inheritdoc />
        protected virtual DbSet<TEntity> Table { get { return Context.Set<TEntity>(); } }

        /// <inheritdoc />
        public EfAsyncRepositoryBase()
        {
            DbContextProvider = DefaultContextProvider<TDbContext>.Instance;
        }

        /// <inheritdoc />
        public EfAsyncRepositoryBase(Func<TDbContext> dbContextFactory)
        {
            DbContextProvider = new FactoryContextProvider<TDbContext>(dbContextFactory);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetAll()
        {
            return await GetAll(CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<TEntity>> GetAll(CancellationToken cancellationToken)
        {
            return await Table.ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<List<TEntity>> GetAllList()
        {
            return await GetAllList(CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task<List<TEntity>> GetAllList(CancellationToken cancellationToken)
        {
            return await Table.ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<List<TEntity>> GetAllList(Expression<Func<TEntity, bool>> predicate)
        {
            return await Table.Where(predicate).ToListAsync(CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task<List<TEntity>> GetAllList(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await Table.Where(predicate).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<T> Query<T>(Func<IQueryable<TEntity>, T> queryMethod)
        {
            return await Task.FromResult(queryMethod(Table));
        }

        /// <inheritdoc />
        public virtual async Task<T> Query<T>(Func<IQueryable<TEntity>, T> queryMethod, CancellationToken cancellationToken)
        {
            return await Task.FromResult(queryMethod(Table));
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> Get(TPrimaryKey id)
        {
            return await Get(id, CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> Get(TPrimaryKey id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var entity = await FirstOrDefault(id, cancellationToken);

            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (entity == null)
            {
                throw new AbpException("There is no such an entity with given primary key. Entity type: " + typeof(TEntity).FullName + ", primary key: " + id);
            }

            return await Task.FromResult(entity);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> Single(Expression<Func<TEntity, bool>> predicate)
        {
            return await Single(predicate, CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> Single(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Table.SingleAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> FirstOrDefault(TPrimaryKey id)
        {
            return await Table.FirstOrDefaultAsync(CreateEqualityExpression(id), CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> FirstOrDefault(TPrimaryKey id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Table.FirstOrDefaultAsync(CreateEqualityExpression(id), cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return await FirstOrDefault(predicate, CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> FirstOrDefault(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Table.FirstOrDefaultAsync(predicate, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> Load(TPrimaryKey id)
        {
            return await Load(id, CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> Load(TPrimaryKey id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Get(id, cancellationToken); //EntityFramework has no Load as like NHibernate.
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> Insert(TEntity entity)
        {
            return await Insert(entity, CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> Insert(TEntity entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Task.FromResult(Table.Add(entity));
        }

        /// <inheritdoc />
        public async Task<TPrimaryKey> InsertAndGetId(TEntity entity)
        {
            return await InsertAndGetId(entity, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task<TPrimaryKey> InsertAndGetId(TEntity entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            entity = await Insert(entity);

            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (EqualityComparer<TPrimaryKey>.Default.Equals(entity.Id, default(TPrimaryKey)))
            {
                // TODO: ammachado: Remove this dependency
                await Task.Run(() => IocManager.Instance.Resolve<UnitOfWorkScope>().Current.SaveChanges(), cancellationToken);
            }

            return await Task.FromResult(entity.Id);
        }

        /// <inheritdoc />
        public async Task<TEntity> InsertOrUpdate(TEntity entity)
        {
            return await InsertOrUpdate(entity, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task<TEntity> InsertOrUpdate(TEntity entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await (EqualityComparer<TPrimaryKey>.Default.Equals(entity.Id, default(TPrimaryKey))
                ? Insert(entity, cancellationToken)
                : Update(entity, cancellationToken));
        }

        /// <inheritdoc />
        public async Task<TPrimaryKey> InsertOrUpdateAndGetId(TEntity entity)
        {
            return await InsertOrUpdateAndGetId(entity, CancellationToken.None);
        }

        /// <inheritdoc />
        public async Task<TPrimaryKey> InsertOrUpdateAndGetId(TEntity entity, CancellationToken cancellationToken)
        {
            entity = await InsertOrUpdate(entity);

            if (EqualityComparer<TPrimaryKey>.Default.Equals(entity.Id, default(TPrimaryKey)))
            {
                // TODO: ammachado: Remove this dependency
                await Task.Run(() => IocManager.Instance.Resolve<UnitOfWorkScope>().Current.SaveChanges(), cancellationToken);
            }

            return await Task.FromResult(entity.Id);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> Update(TEntity entity)
        {
            return await Update(entity, CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task<TEntity> Update(TEntity entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Task.Run(() =>
            {
                Table.Attach(entity);
                Context.Entry(entity).State = EntityState.Modified;
                return entity;
            }, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task Delete(TEntity entity)
        {
            await Delete(entity, CancellationToken.None);
        }

        public virtual async Task Delete(TEntity entity, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await Task.Run(() =>
            {
                if (entity is ISoftDelete)
                {
                    (entity as ISoftDelete).IsDeleted = true;
                }
                else
                {
                    Table.Remove(entity);
                }
            }, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task Delete(TPrimaryKey id)
        {
            await Delete(id, CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task Delete(TPrimaryKey id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var entity = Table.Local.FirstOrDefault(ent => EqualityComparer<TPrimaryKey>.Default.Equals(ent.Id, id));
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            if (entity == null)
            {
                entity = await FirstOrDefault(id, cancellationToken);
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                if (entity == null)
                {
                    return;
                }
            }

            await Delete(entity, cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task Delete(Expression<Func<TEntity, bool>> predicate)
        {
            await Delete(predicate, CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task Delete(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var entities = await Table.Where(predicate).ToListAsync(cancellationToken);
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            foreach (var entity in entities)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                }

                await Delete(entity, cancellationToken);
            }
        }

        /// <inheritdoc />
        public virtual async Task<int> Count()
        {
            return await Count(CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task<int> Count(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Table.CountAsync();
        }

        /// <inheritdoc />
        public virtual async Task<int> Count(Expression<Func<TEntity, bool>> predicate)
        {
            return await Count(predicate, CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task<int> Count(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Table.CountAsync(predicate);
        }

        /// <inheritdoc />
        public virtual async Task<long> LongCount()
        {
            return await LongCount(CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task<long> LongCount(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Table.LongCountAsync(cancellationToken);
        }

        /// <inheritdoc />
        public virtual async Task<long> LongCount(Expression<Func<TEntity, bool>> predicate)
        {
            return await Table.LongCountAsync(predicate, CancellationToken.None);
        }

        /// <inheritdoc />
        public virtual async Task<long> LongCount(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return await Table.LongCountAsync(predicate, cancellationToken);
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
