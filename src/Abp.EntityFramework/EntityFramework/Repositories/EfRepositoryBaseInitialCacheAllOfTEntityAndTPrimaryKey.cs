//--------------------------------------------------------------------------------------------------------------------------------
// 功能描述：实现领域对象在仓储层的缓存，适用于缓存数据库指定表中某租户相关的所有数据，比如某租户的所有用户数据、角色数据等。
// 组  织：AndHuang
// 创建者：Andrew		创建日期：20150911
// 修改者：Andrew		修改日期：20150922
// 修改描述：List<T>实现 ---> ConcurrentDictionary<TKey, TValue>实现
//--------------------------------------------------------------------------------------------------------------------------------
//--------------------------------------------------------------------------------------------------------------------------------
// Description：Cache domain objects in repository layer, fit for caching tenant's data, such as user role etc
// Organization：AndHuang
// Creator：Andrew		Date：20150911
// Modifier：Andrew		Date：20150922
// Modify description：List<T> ---> ConcurrentDictionary<TKey, TValue>
//--------------------------------------------------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFramework;
using Abp.Runtime.Caching;
using EntityFramework.DynamicFilters;
using Abp.Domain.Uow;
using System.Text;
using Abp.EntityFramework.FakeDbAsync;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Metadata.Edm;
using System.Collections.Concurrent;

namespace Abp.EntityFramework.Repositories
{
    /// <summary>
    /// Implements IRepository for Entity Framework.
    /// </summary>
    /// <typeparam name="TDbContext">DbContext which contains <see cref="TEntity"/>.</typeparam>
    /// <typeparam name="TEntity">Type of the Entity for this repository</typeparam>
    /// <typeparam name="TPrimaryKey">Primary key of the entity</typeparam>
    public class EfRepositoryBaseInitialCacheAll<TDbContext, TEntity, TPrimaryKey> : AbpRepositoryBase<TEntity, TPrimaryKey>, IFromDB<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TDbContext : AbpDbContext
    {
        private readonly ICacheManager _cacheManager;
        /// <summary>
        /// Gets EF DbContext object.
        /// </summary>
        protected virtual TDbContext Context { get { return _dbContextProvider.DbContext; } }

        /// <summary>
        /// Gets DbSet for given entity.
        /// </summary>
        protected virtual DbSet<TEntity> Table { get { return Context.Set<TEntity>(); } }

        private readonly IDbContextProvider<TDbContext> _dbContextProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dbContextProvider"></param>
        public EfRepositoryBaseInitialCacheAll(IDbContextProvider<TDbContext> dbContextProvider, ICacheManager cacheManager)
        {
            _dbContextProvider = dbContextProvider;
            _cacheManager = cacheManager;
        }

        /// <summary>
        /// 20150921HW获取CacheStoreName
        /// </summary>
        public string GetCacheStoreName()
        {
            return "Tenant" + (Context.AbpSession.TenantId ?? 0).ToString();
        }

        /// <summary>
        /// 20150915HW获取TypedCache
        /// </summary>
        public ITypedCache<TKey, TValue> GetTypedCache<TKey, TValue>()
        {
            return _cacheManager.GetCache<TKey, TValue>(GetCacheStoreName());
        }

        /// <summary>
        /// 20150915HW获取key
        /// </summary>
        public string GetKey()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(typeof(TEntity).FullName);
            stringBuilder.Append(' ');
            if (Context.IsFilterEnabled(AbpDataFilters.MustHaveTenant) && typeof(IMustHaveTenant).IsAssignableFrom(typeof(TEntity)))
            {
                stringBuilder.Append(AbpDataFilters.MustHaveTenant);
                stringBuilder.Append(Context.GetFilterParameterValue(AbpDataFilters.MustHaveTenant, AbpDataFilters.Parameters.TenantId));
            }
            if (Context.IsFilterEnabled(AbpDataFilters.MayHaveTenant) && typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity)))
            {
                stringBuilder.Append(AbpDataFilters.MayHaveTenant);
                stringBuilder.Append(Context.GetFilterParameterValue(AbpDataFilters.MayHaveTenant, AbpDataFilters.Parameters.TenantId));
            }
            if (Context.IsFilterEnabled(AbpDataFilters.SoftDelete) && typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                stringBuilder.Append(AbpDataFilters.SoftDelete);
                stringBuilder.Append(Context.GetFilterParameterValue(AbpDataFilters.SoftDelete, AbpDataFilters.Parameters.IsDeleted));
            }
            return stringBuilder.ToString();
        }

        /// <summary>
        /// 20150915HW从数据库中读取所有关联表的数据。
        /// </summary>
        public virtual IQueryable<TEntity> TableIncludeAll(Expression<Func<TEntity, bool>> where = null)
        {
            IQueryable<TEntity> query = where == null ? this.Context.Set<TEntity>() : this.Context.Set<TEntity>().Where(where);

            var workspace = ((IObjectContextAdapter)this.Context).ObjectContext.MetadataWorkspace;
            var itemCollection = (ObjectItemCollection)(workspace.GetItemCollection(DataSpace.OSpace));
            var entityType = itemCollection.OfType<EntityType>().FirstOrDefault(e => itemCollection.GetClrType(e) == typeof(TEntity));

            if (entityType != null)
            {
                foreach (var navigationProperty in entityType.NavigationProperties)
                {
                    query = query.Include(navigationProperty.Name);
                }
            }

            return query;
        }

        /// <summary>
        /// 20150915HW获取缓存中的数据，如果缓存为空，则设置缓存。
        /// </summary>
        public ConcurrentDictionary<TPrimaryKey, TEntity> GetAllListFromCache()
        {
            var typedCache = GetTypedCache<string, ConcurrentDictionary<TPrimaryKey, TEntity>>();
            string key = GetKey();
            ConcurrentDictionary<TPrimaryKey, TEntity> dictionary = typedCache.GetOrDefault(key);
            if (dictionary == null)
            {
                dictionary = new ConcurrentDictionary<TPrimaryKey, TEntity>();
                foreach (TEntity entity in TableIncludeAll().AsEnumerable<TEntity>())
                {
                    dictionary.AddOrUpdate(entity.Id, entity, (k, oldValue) => entity);
                }

                typedCache.Set(key, dictionary);
            }
            return dictionary;
        }

        public override IQueryable<TEntity> GetAll()
        {
            var fakeAsynEnumerable = new FakeDbAsyncEnumerable<TEntity>(GetAllListFromCache().Values);
            return fakeAsynEnumerable.AsQueryable();
        }

        public IQueryable<TEntity> GetAllFromDB()
        {
            return Table;
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
            TEntity entity;
            GetAllListFromCache().TryGetValue(id, out entity);
            return await Task.FromResult(entity);////////////
        }

        public async Task<TEntity> FirstOrDefaultAsyncFromDB(TPrimaryKey id)
        {
            return await Table.FirstOrDefaultAsync(CreateEqualityExpressionForId(id));
        }

        public override async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await GetAll().FirstOrDefaultAsync(predicate);
        }

        public override TEntity Insert(TEntity entity)
        {
            TEntity returnEntity = Table.Add(entity);
            ConcurrentDictionary<TPrimaryKey, TEntity> dictionary = GetAllListFromCache();
            dictionary.AddOrUpdate(returnEntity.Id, returnEntity, (key, oldValue) => returnEntity);
            return returnEntity;
        }

        public override Task<TEntity> InsertAsync(TEntity entity)
        {
            return Task.FromResult(Insert(entity));
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

        /// <summary>
        /// 20150915HW将实体从缓存中删除。
        /// </summary>
        public bool RemoveEntityInCache(TEntity entity)
        {
            ConcurrentDictionary<TPrimaryKey, TEntity> dictionary = GetAllListFromCache();
            TEntity removedEntity;
            return dictionary.TryRemove(entity.Id, out removedEntity);
        }

        /// <summary>
        /// 20150915HW更新缓存中的实体数据，如果没有该实体，则将该实体添加到缓存中去。
        /// </summary>
        public void AddOrUpdateEntityInCache(TEntity entity)
        {
            ConcurrentDictionary<TPrimaryKey, TEntity> dictionary = GetAllListFromCache();
            dictionary.AddOrUpdate(entity.Id, entity, (key, oldValue) => entity);
        }

        public override TEntity Update(TEntity entity)
        {
            AttachIfNot(entity);
            Context.Entry(entity).State = EntityState.Modified;

            AddOrUpdateEntityInCache(entity);//20150915

            return entity;
        }

        public override Task<TEntity> UpdateAsync(TEntity entity)
        {
            return Task.FromResult(Update(entity));
        }

        public override void Delete(TEntity entity)
        {
            AttachIfNot(entity);

            if (entity is ISoftDelete)
            {
                if ((entity as ISoftDelete).IsDeleted == false)
                {
                    (entity as ISoftDelete).IsDeleted = true;
                    AddOrUpdateEntityInCache(entity);//20150915
                }
            }
            else
            {
                Table.Remove(entity);
                RemoveEntityInCache(entity);//20150915
            }
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

        /// <summary>
        /// 20150915HW回收已删除的实体。
        /// </summary>
        public void Undelete(TEntity entity)
        {
            AttachIfNot(entity);

            if (entity is ISoftDelete)
            {
                if ((entity as ISoftDelete).IsDeleted == true)
                {
                    (entity as ISoftDelete).IsDeleted = false;
                    AddOrUpdateEntityInCache(entity);//20150915
                }
            }
        }

        /// <summary>
        /// 20150915HW回收已删除的实体。
        /// </summary>
        public void Undelete(TPrimaryKey id)
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

            Undelete(entity);
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

        protected virtual void AttachIfNot(TEntity entity)
        {
            if (!Table.Local.Contains(entity))
            {
                Table.Attach(entity);
            }
        }
    }
}
