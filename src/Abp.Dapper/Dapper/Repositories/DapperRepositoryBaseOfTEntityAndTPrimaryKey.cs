using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Abp.Dapper.Extensions;
using Abp.Dapper.Filters.Action;
using Abp.Dapper.Filters.Query;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.EntityFramework;
using Abp.Events.Bus.Entities;

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
            EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
            DapperQueryFilterExecuter = NullDapperQueryFilterExecuter.Instance;
            DapperActionFilterExecuter = NullDapperActionFilterExecuter.Instance;
        }

        public IDapperQueryFilterExecuter DapperQueryFilterExecuter { get; set; }

        public IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

        public IDapperActionFilterExecuter DapperActionFilterExecuter { get; set; }

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
        ///     and there must be an active transaction.
        /// </summary>
        /// <value>
        ///     The active transaction.
        /// </value>
        public virtual IDbTransaction ActiveTransaction
        {
            get { return Context.Database.CurrentTransaction.UnderlyingTransaction; }
        }

        public override TEntity Single(TPrimaryKey id)
        {
            return Single(CreateEqualityExpressionForId(id));
        }

        public override TEntity Single(Expression<Func<TEntity, bool>> predicate)
        {
            IPredicate pg = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            return Connection.GetList<TEntity>(pg, transaction: ActiveTransaction).Single();
        }

        public override TEntity FirstOrDefault(TPrimaryKey id)
        {
            return FirstOrDefault(CreateEqualityExpressionForId(id));
        }

        public override TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            IPredicate pg = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            return Connection.GetList<TEntity>(pg, transaction: ActiveTransaction).FirstOrDefault();
        }

        public override TEntity Get(TPrimaryKey id)
        {
            TEntity item = FirstOrDefault(id);
            if (item == null) { throw new EntityNotFoundException(typeof(TEntity), id); }

            return item;
        }

        public override IEnumerable<TEntity> GetAll()
        {
            PredicateGroup predicateGroup = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>();
            return Connection.GetList<TEntity>(predicateGroup, transaction: ActiveTransaction);
        }

        public override Task<IEnumerable<TEntity>> GetAllAsync()
        {
            PredicateGroup predicateGroup = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>();
            return Connection.GetListAsync<TEntity>(predicateGroup, transaction: ActiveTransaction);
        }

        public override IEnumerable<TEntity> Query(string query, object parameters = null)
        {
            return Connection.Query<TEntity>(query, parameters, ActiveTransaction);
        }

        public override Task<IEnumerable<TEntity>> QueryAsync(string query, object parameters = null)
        {
            return Connection.QueryAsync<TEntity>(query, parameters, ActiveTransaction);
        }

        public override IEnumerable<TAny> Query<TAny>(string query, object parameters = null)
        {
            return Connection.Query<TAny>(query, parameters, ActiveTransaction);
        }

        public override Task<IEnumerable<TAny>> QueryAsync<TAny>(string query, object parameters = null)
        {
            return Connection.QueryAsync<TAny>(query, parameters, ActiveTransaction);
        }

        public override IEnumerable<TEntity> GetAllPaged(Expression<Func<TEntity, bool>> predicate, int pageNumber, int itemsPerPage, string sortingProperty, bool ascending = true)
        {
            IPredicate filteredPredicate = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);

            return Connection.GetPage<TEntity>(
                filteredPredicate,
                new List<ISort> { new Sort { Ascending = ascending, PropertyName = sortingProperty } },
                pageNumber,
                itemsPerPage,
                ActiveTransaction);
        }

        public override int Count(Expression<Func<TEntity, bool>> predicate)
        {
            IPredicate filteredPredicate = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            return Connection.Count<TEntity>(filteredPredicate, ActiveTransaction);
        }

        public override IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults, string sortingProperty, bool ascending = true)
        {
            IPredicate filteredPredicate = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            return Connection.GetSet<TEntity>(
                filteredPredicate,
                new List<ISort> { new Sort { Ascending = ascending, PropertyName = sortingProperty } },
                firstResult,
                maxResults,
                ActiveTransaction
            );
        }

        public override IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            IPredicate filteredPredicate = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            return Connection.GetList<TEntity>(filteredPredicate, transaction: ActiveTransaction);
        }

        public override Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            IPredicate filteredPredicate = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            return Connection.GetListAsync<TEntity>(filteredPredicate, transaction: ActiveTransaction);
        }

        public override Task<IEnumerable<TEntity>> GetSetAsync(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults, string sortingProperty, bool ascending = true)
        {
            IPredicate filteredPredicate = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            return Connection.GetSetAsync<TEntity>(
                filteredPredicate,
                new List<ISort> { new Sort { Ascending = ascending, PropertyName = sortingProperty } },
                firstResult,
                maxResults,
                ActiveTransaction
            );
        }

        public override Task<IEnumerable<TEntity>> GetAllPagedAsync(Expression<Func<TEntity, bool>> predicate, int pageNumber, int itemsPerPage, string sortingProperty, bool ascending = true)
        {
            IPredicate filteredPredicate = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            return Connection.GetPageAsync<TEntity>(
                filteredPredicate,
                new List<ISort> { new Sort { Ascending = ascending, PropertyName = sortingProperty } },
                pageNumber,
                itemsPerPage,
                ActiveTransaction);
        }

        public override Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            IPredicate filteredPredicate = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            return Connection.CountAsync<TEntity>(filteredPredicate, ActiveTransaction);
        }

        public override IEnumerable<TEntity> GetAllPaged(Expression<Func<TEntity, bool>> predicate, int pageNumber, int itemsPerPage, bool ascending = true, params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            IPredicate filteredPredicate = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            return Connection.GetPage<TEntity>(filteredPredicate, sortingExpression.ToSortable(ascending), pageNumber, itemsPerPage, ActiveTransaction);
        }

        public override IEnumerable<TEntity> GetSet(Expression<Func<TEntity, bool>> predicate, int firstResult, int maxResults, bool ascending = true, params Expression<Func<TEntity, object>>[] sortingExpression)
        {
            IPredicate filteredPredicate = DapperQueryFilterExecuter.ExecuteFilter<TEntity, TPrimaryKey>(predicate);
            return Connection.GetSet<TEntity>(filteredPredicate, sortingExpression.ToSortable(ascending), firstResult, maxResults, ActiveTransaction);
        }

        public override void Insert(TEntity entity)
        {
            InsertAndGetId(entity);
        }

        public override void Update(TEntity entity)
        {
            EntityChangeEventHelper.TriggerEntityUpdatingEvent(entity);
            DapperActionFilterExecuter.ExecuteModificationAuditFilter<TEntity, TPrimaryKey>(entity);
            Connection.Update(entity, ActiveTransaction);
            EntityChangeEventHelper.TriggerEntityUpdatedEventOnUowCompleted(entity);
        }

        public override void Delete(TEntity entity)
        {
            EntityChangeEventHelper.TriggerEntityDeletingEvent(entity);
            if (entity is ISoftDelete)
            {
                DapperActionFilterExecuter.ExecuteDeletionAuditFilter<TEntity, TPrimaryKey>(entity);
                Connection.Update(entity, ActiveTransaction);
            }
            else
            {
                Connection.Delete(entity, ActiveTransaction);
            }
            EntityChangeEventHelper.TriggerEntityDeletedEventOnUowCompleted(entity);
        }

        public override void Delete(Expression<Func<TEntity, bool>> predicate)
        {
            IEnumerable<TEntity> items = GetAll(predicate);
            foreach (TEntity entity in items)
            {
                Delete(entity);
            }
        }

        public override TPrimaryKey InsertAndGetId(TEntity entity)
        {
            EntityChangeEventHelper.TriggerEntityCreatingEvent(entity);
            DapperActionFilterExecuter.ExecuteCreationAuditFilter<TEntity, TPrimaryKey>(entity);
            TPrimaryKey primaryKey = Connection.Insert(entity, ActiveTransaction);
            EntityChangeEventHelper.TriggerEntityCreatedEventOnUowCompleted(entity);
            return primaryKey;
        }
    }
}
