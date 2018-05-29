using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Abp.Application.Services.Dto;
using Abp.Collections.Extensions;
using Abp.Data;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;

namespace Abp.EntityFramework.Repositories
{
    /// <summary>
    /// Implements IRepository for Entity Framework.
    /// </summary>
    /// <typeparam name="TDbContext">DbContext which contains <typeparamref name="TEntity"/>.</typeparam>
    /// <typeparam name="TEntity">Type of the Entity for this repository</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key of the entity</typeparam>
    public class EfRepositoryBase<TDbContext, TEntity, TPrimaryKey> : AbpRepositoryBase<TEntity, TPrimaryKey>, IRepositoryWithDbContext, ISupportsExplicitLoading<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TDbContext : DbContext
    {
        /// <summary>
        /// Gets EF DbContext object.
        /// </summary>
        public virtual TDbContext Context => _dbContextProvider.GetDbContext(MultiTenancySide);

        /// <summary>
        /// Gets DbSet for given entity.
        /// </summary>
        public virtual DbSet<TEntity> Table => Context.Set<TEntity>();

        public virtual DbTransaction Transaction
        {
            get
            {
                return (DbTransaction)TransactionProvider?.GetActiveTransaction(new ActiveTransactionProviderArgs
                {
                    {"ContextType", typeof(TDbContext) },
                    {"MultiTenancySide", MultiTenancySide }
                });
            }
        }

        public virtual DbConnection Connection
        {
            get
            {
                var connection = Context.Database.Connection;

                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                return connection;
            }
        }

        private readonly IDbContextProvider<TDbContext> _dbContextProvider;

        public IActiveTransactionProvider TransactionProvider { private get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContextProvider"></param>
        public EfRepositoryBase(IDbContextProvider<TDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public override IQueryable<TEntity> GetAll()
        {
            return Table;
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
                query = query.Include(propertySelector);
            }

            return query;
        }

        public override async Task<List<TEntity>> GetAllListAsync()
        {
            return await GetAll().ToListAsync();
        }

        public override async Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).ToListAsync();
        }

        public override async Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().SingleAsync(predicate);
        }

        public override async Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id)
        {
            return await GetAll().FirstOrDefaultAsync(CreateEqualityExpressionForId(id));
        }

        public override async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().FirstOrDefaultAsync(predicate);
        }

        public override TEntity Insert(TEntity entity)
        {
            return Table.Add(entity);
        }

        public override Task<TEntity> InsertAsync(TEntity entity)
        {
            return Task.FromResult(Table.Add(entity));
        }

        public override TPrimaryKey InsertAndGetId(TEntity entity)
        {
            entity = Insert(entity);

            if (entity.IsTransient())
            {
                Context.SaveChanges();
            }

            return entity.Id;
        }

        public override async Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity)
        {
            entity = await InsertAsync(entity);

            if (entity.IsTransient())
            {
                await Context.SaveChangesAsync();
            }

            return entity.Id;
        }

        public override TPrimaryKey InsertOrUpdateAndGetId(TEntity entity)
        {
            entity = InsertOrUpdate(entity);

            if (entity.IsTransient())
            {
                Context.SaveChanges();
            }

            return entity.Id;
        }

        public override async Task<TPrimaryKey> InsertOrUpdateAndGetIdAsync(TEntity entity)
        {
            entity = await InsertOrUpdateAsync(entity);

            if (entity.IsTransient())
            {
                await Context.SaveChangesAsync();
            }

            return entity.Id;
        }

        public override TEntity Update(TEntity entity)
        {
            AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Modified;
            return entity;
        }

        public override Task<TEntity> UpdateAsync(TEntity entity)
        {
            AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Modified;
            return Task.FromResult(entity);
        }

        public override void Delete(TEntity entity)
        {
            AttachIfNot(entity);
            Table.Remove(entity);
        }

        public override void Delete(TPrimaryKey id)
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

        public override async Task<int> CountAsync()
        {
            return await GetAll().CountAsync();
        }

        public override async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).CountAsync();
        }

        public override async Task<long> LongCountAsync()
        {
            return await GetAll().LongCountAsync();
        }

        public override async Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).LongCountAsync();
        }

        public bool Exist(TPrimaryKey pId)
        {
            List<TPrimaryKey> list = new List<TPrimaryKey>()
            {
                pId
            };

            return GetAll().Any(a => list.Contains(a.Id));
        }

        public async Task<bool> ExistAsync(TPrimaryKey pId)
        {
            List<TPrimaryKey> list = new List<TPrimaryKey>()
            {
                pId
            };

            return await GetAll().AnyAsync(a => list.Contains(a.Id));
        }

        public bool Exists(List<TPrimaryKey> pIds)
        {
            int count = GetAll().Count(a => pIds.Contains(a.Id));

            return count == pIds.Count;
        }

        public async Task<bool> ExistsAsync(List<TPrimaryKey> pIds)
        {
            int count = await GetAll().CountAsync(a => pIds.Contains(a.Id));

            return count == pIds.Count;
        }

        public List<TPrimaryKey> ExistsReturnUnexistingIds(List<TPrimaryKey> ids)
        {
            List<TPrimaryKey> existingIds = GetAll().Where(x => ids.Contains(x.Id)).Select(x => x.Id).ToList();

            List<TPrimaryKey> unexistingIds = ids.Except(existingIds).ToList();

            return unexistingIds;
        }

        public async Task<List<TPrimaryKey>> ExistsReturnIdsAsync(List<TPrimaryKey> ids)
        {
            List<TPrimaryKey> existingIds = await GetAll().Where(x => ids.Contains(x.Id)).Select(x => x.Id).ToListAsync();

            List<TPrimaryKey> unexistingIds = ids.Except(existingIds).ToList();

            return unexistingIds;
        }

        public async Task<PagedResultDto<TEntity>> GetPagedListAsync(int? skip, int? take, Func<TEntity, bool> predicate = null)
        {
            IQueryable<TEntity> query = GetAll();

            if (predicate != null)
            {
                query = query.Where(predicate).AsQueryable();
            }

            int count = query.Count();

            query = query.Skip(skip ?? 0)
                         .Take(take ?? (count == 0 ? 1 : count));

            List<TEntity> results = await Task.FromResult(query.ToList());

            return new PagedResultDto<TEntity>(count, results);
        }

        public PagedResultDto<TEntity> GetPagedList(int? pSkip, int? pTake, Func<TEntity, bool> predicate = null)
        {
            IQueryable<TEntity> query = GetAll();

            if (predicate != null)
            {
                query = query.Where(predicate).AsQueryable();
            }

            int count = query.Count();

            query = query.Skip(pSkip ?? 0)
                         .Take(pTake ?? (count == 0 ? 1 : count));

            List<TEntity> results = query.ToList();

            return new PagedResultDto<TEntity>(count, results);
        }

        public async Task<PagedResultDto<TEntity>> GetPagedListWithTranslationsAsync<TTranslated, TTranslation>(int? skip, int? take, Func<TTranslated, bool> predicate = null)
            where TTranslated : class, TEntity, IMultiLingualEntity<TTranslation>
            where TTranslation : class, IEntityTranslation
        {
            return await Task.FromResult(GetPagedListWithTranslations<TTranslated, TTranslation>(skip, take, predicate));
        }

        public PagedResultDto<TEntity> GetPagedListWithTranslations<TTranslated, TTranslation>(int? skip, int? take, Func<TTranslated, bool> predicate = null)
            where TTranslated : class, TEntity, IMultiLingualEntity<TTranslation>
            where TTranslation : class, IEntityTranslation
        {
            IQueryable<TTranslated> query = this.Context.Set<TTranslated>().AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate).AsQueryable();
            }

            int count = query.Count();

            query = query.Skip(skip ?? 0)
                         .Take(take ?? (count == 0 ? 1 : count));

            var projectedQuery = query.Select(s => new
            {
                Entity = s,
                //Because the translations are included in the ef context they will be map automatically inside the entity
                Translations = s.Translations
            }).ToList();

            List<TTranslated> results = projectedQuery.Select(s => s.Entity).ToList();

            return new PagedResultDto<TEntity>(count, results);
        }

        public List<TEntity> GetRange(List<TPrimaryKey> pIds)
        {
            return GetRange(predicate => pIds.Contains(predicate.Id));
        }

        public List<TEntity> GetRange(Expression<Func<TEntity, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public async Task<List<TEntity>> GetRangeAsync(List<TPrimaryKey> pIds)
        {
            return await GetRangeAsync(predicate => pIds.Contains(predicate.Id));
        }

        public async Task<List<TEntity>> GetRangeAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().Where(predicate).ToListAsync();
        }

        protected virtual void AttachIfNot(TEntity entity)
        {
            if (!Table.Local.Contains(entity))
            {
                Table.Attach(entity);
            }
        }

        public DbContext GetDbContext()
        {
            return Context;
        }

        public Task EnsureCollectionLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, IEnumerable<TProperty>>> collectionExpression,
            CancellationToken cancellationToken) where TProperty : class
        {
            var expression = collectionExpression.Body as MemberExpression;
            if (expression == null)
            {
                throw new AbpException($"Given {nameof(collectionExpression)} is not a {typeof(MemberExpression).FullName}");
            }

            return Context.Entry(entity)
                .Collection(expression.Member.Name)
                .LoadAsync(cancellationToken);
        }

        public Task EnsurePropertyLoadedAsync<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> propertyExpression,
            CancellationToken cancellationToken) where TProperty : class
        {
            return Context.Entry(entity).Reference(propertyExpression).LoadAsync(cancellationToken);
        }
    }
}
