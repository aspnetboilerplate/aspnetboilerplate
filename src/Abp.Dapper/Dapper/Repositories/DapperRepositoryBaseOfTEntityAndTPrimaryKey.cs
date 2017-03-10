using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Dapper.Expressions;
using Abp.Dapper.Extensions;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.EntityFramework;
using Abp.EntityFramework.Uow;
using Dapper;
using DapperExtensions;

namespace Abp.Dapper.Repositories
{
    public class DapperRepositoryBase<TDbContext, TEntity, TPrimaryKey> : AbpDapperRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
        where TDbContext : DbContext
    {
        private readonly IDbContextProvider<TDbContext> _dbContextProvider;

        public DapperRepositoryBase(IDbContextProvider<TDbContext> dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;
        }

        public virtual TDbContext Context
        {
            get { return _dbContextProvider.GetDbContext(); }
        }

        public virtual DbConnection Connection
        {
            get { return Context.Database.Connection; }
        }

        /// <summary>
        ///     Gets the active transaction. If Dapper is active then <see cref="IUnitOfWork" /> should be started before
        ///     and there must be an active transaction. To Activate Dapper Use <see cref="DbContextEfTransactionStrategy" />
        /// </summary>
        /// <value>
        ///     The active transaction.
        /// </value>
        public virtual IDbTransaction ActiveTransaction
        {
            get { return Context.Database.CurrentTransaction.UnderlyingTransaction; }
        }

        public override TEntity Get(TPrimaryKey id)
        {
            return Connection.Get<TEntity>(id, ActiveTransaction);
        }

        public override Task<TEntity> GetAsync(TPrimaryKey id)
        {
            return Connection.GetAsync<TEntity>(id, ActiveTransaction);
        }

        public override IEnumerable<TEntity> GetList()
        {
            return Connection.GetList<TEntity>(transaction: ActiveTransaction);
        }

        public override IEnumerable<TEntity> GetList(object predicate)
        {
            return Connection.GetList<TEntity>(predicate, transaction: ActiveTransaction);
        }

        public override Task<IEnumerable<TEntity>> GetListAsync()
        {
            return Connection.GetListAsync<TEntity>(transaction: ActiveTransaction);
        }

        public override Task<IEnumerable<TEntity>> GetListAsync(object predicate)
        {
            return Connection.GetListAsync<TEntity>(predicate, transaction: ActiveTransaction);
        }

        public override IEnumerable<TEntity> GetListPaged(
            object predicate,
            int pageNumber,
            int itemsPerPage,
            string sortingProperty,
            bool ascending = true)
        {
            return Connection.GetPage<TEntity>(
                predicate,
                new List<ISort> { new Sort { Ascending = ascending, PropertyName = sortingProperty } },
                pageNumber,
                itemsPerPage,
                ActiveTransaction);
        }

        public override Task<IEnumerable<TEntity>> GetListPagedAsync(
            object predicate,
            int pageNumber,
            int itemsPerPage,
            string sortingProperty,
            bool ascending = true)
        {
            return Connection.GetPageAsync<TEntity>(
                predicate,
                new List<ISort> { new Sort { Ascending = ascending, PropertyName = sortingProperty } },
                pageNumber,
                itemsPerPage,
                ActiveTransaction);
        }

        public override int Count(object predicate)
        {
            return Connection.Count<TEntity>(predicate, ActiveTransaction);
        }

        public override Task<int> CountAsync(object predicate)
        {
            return Connection.CountAsync<TEntity>(predicate, ActiveTransaction);
        }

        public override IEnumerable<TEntity> Query(string query, object parameters)
        {
            return Connection.Query<TEntity>(query, parameters, ActiveTransaction);
        }

        public override Task<IEnumerable<TEntity>> QueryAsync(string query, object parameters)
        {
            return Connection.QueryAsync<TEntity>(query, parameters, ActiveTransaction);
        }

        public override IEnumerable<TAny> Query<TAny>(string query)
        {
            return Connection.Query<TAny>(query, transaction: ActiveTransaction);
        }

        public override Task<IEnumerable<TAny>> QueryAsync<TAny>(string query)
        {
            return Connection.QueryAsync<TAny>(query, transaction: ActiveTransaction);
        }

        public override IEnumerable<TAny> Query<TAny>(string query, object parameters)
        {
            return Connection.Query<TAny>(query, parameters, ActiveTransaction);
        }

        public override Task<IEnumerable<TAny>> QueryAsync<TAny>(string query, object parameters)
        {
            return Connection.QueryAsync<TAny>(query, parameters, ActiveTransaction);
        }

        public override IEnumerable<TEntity> GetSet(object predicate, int firstResult, int maxResults, string sortingProperty, bool ascending = true)
        {
            return Connection.GetSet<TEntity>(
                predicate,
                new List<ISort> { new Sort { Ascending = ascending, PropertyName = sortingProperty } },
                firstResult,
                maxResults,
                ActiveTransaction);
        }

        public override Task<IEnumerable<TEntity>> GetSetAsync(object predicate, int firstResult, int maxResults, string sortingProperty, bool ascending = true)
        {
            return Connection.GetSetAsync<TEntity>(
                predicate,
                new List<ISort> { new Sort { Ascending = ascending, PropertyName = sortingProperty } },
                firstResult,
                maxResults,
                ActiveTransaction);
        }

        public override IEnumerable<TEntity> GetListPaged(Expression<Func<TEntity, bool>> predicate, int pageNumber, int itemsPerPage, string sortingProperty, bool ascending = true)
        {
            return Connection.GetPage<TEntity>(
                predicate.ToPredicateGroup<TEntity, TPrimaryKey>(),
                new List<ISort> { new Sort { Ascending = ascending, PropertyName = sortingProperty } },
                pageNumber,
                itemsPerPage,
                ActiveTransaction);
        }

        public override int Count(Expression<Func<TEntity, bool>> predicate)
        {
            return Connection.Count<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>(), ActiveTransaction);
        }

        public override IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults, string sortingProperty, bool ascending = true)
        {
            return Connection.GetSet<TEntity>(
                predicate.ToPredicateGroup<TEntity, TPrimaryKey>(),
                new List<ISort> { new Sort { Ascending = ascending, PropertyName = sortingProperty } },
                firstResult,
                maxResults,
                ActiveTransaction
            );
        }

        public override IEnumerable<TEntity> GetList(Expression<Func<TEntity, bool>> predicate)
        {
            return Connection.GetList<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>(), transaction: ActiveTransaction);
        }

        public override Task<IEnumerable<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Connection.GetListAsync<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>(), transaction: ActiveTransaction);
        }

        public override Task<IEnumerable<TEntity>> GetSetAsync(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults, string sortingProperty, bool ascending = true)
        {
            return Connection.GetSetAsync<TEntity>(
                predicate.ToPredicateGroup<TEntity, TPrimaryKey>(),
                new List<ISort> { new Sort { Ascending = ascending, PropertyName = sortingProperty } },
                firstResult,
                maxResults,
                ActiveTransaction
            );
        }

        public override Task<IEnumerable<TEntity>> GetListPagedAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int itemsPerPage, string sortingProperty, bool ascending = true)
        {
            return Connection.GetPageAsync<TEntity>(
                predicate.ToPredicateGroup<TEntity, TPrimaryKey>(),
                new List<ISort> { new Sort { Ascending = ascending, PropertyName = sortingProperty } },
                pageNumber,
                itemsPerPage,
                ActiveTransaction);
        }

        public override Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Connection.CountAsync<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>(), ActiveTransaction);
        }

        public override IEnumerable<TEntity> GetListPaged(Expression<Func<TEntity, bool>> predicate, int pageNumber, int itemsPerPage, bool ascending = true, params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            return Connection.GetPage<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>(), sortingExpression.ToSortable(ascending), pageNumber, itemsPerPage, ActiveTransaction);
        }

        public override IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults, bool ascending = true, params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            return Connection.GetSet<TEntity>(predicate.ToPredicateGroup<TEntity, TPrimaryKey>(), sortingExpression.ToSortable(ascending), firstResult, maxResults, ActiveTransaction);
        }

        public override void Insert(TEntity entity)
        {
            Connection.Insert(entity, ActiveTransaction);
        }

        public override void Update(TEntity entity)
        {
            Connection.Update(entity, ActiveTransaction);
        }

        public override void Delete(TEntity entity)
        {
            Connection.Delete(entity, ActiveTransaction);
        }

        public override void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            Connection.Delete(predicate.ToPredicateGroup<TEntity, TPrimaryKey>(), ActiveTransaction);
        }

        public override TPrimaryKey InsertAndGetId(TEntity entity)
        {
            return Connection.Insert(entity, ActiveTransaction);
        }
    }
}
