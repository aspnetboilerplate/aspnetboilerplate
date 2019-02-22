using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using Abp.Runtime.Session;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Abp.EntityFrameworkCore.EFPlus
{
    /// <summary>
    /// Defines batch delete and update extension methods for IRepository
    /// </summary>
    public static class AbpEntityFrameworkCoreEfPlusExtensions
    {
        /// <summary>
        /// Deletes all matching entities permanently for given predicate
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TPrimaryKey">Primary key type</typeparam>
        /// <param name="repository">Repository</param>
        /// <param name="predicate">Predicate to filter entities</param>
        /// <returns></returns>
        public static async Task<int> BatchDeleteAsync<TEntity, TPrimaryKey>([NotNull] this IRepository<TEntity, TPrimaryKey> repository, [NotNull] Expression<Func<TEntity, bool>> predicate)
            where TEntity : Entity<TPrimaryKey>
        {
            Check.NotNull(repository, nameof(repository));
            Check.NotNull(predicate, nameof(predicate));

            var query = repository.GetAll().IgnoreQueryFilters();

            var abpFilterExpression = GetFilterExpressionOrNull<TEntity, TPrimaryKey>(repository.GetIocResolver());
            var filterExpression = ExpressionCombiner.Combine(predicate, abpFilterExpression);

            query = query.Where(filterExpression);

            return await query.DeleteAsync();
        }

        /// <summary>
        /// Deletes all matching entities permanently for given predicate
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="repository">Repository</param>
        /// <param name="predicate">Predicate to filter entities</param>
        /// <returns></returns>
        public static async Task<int> BatchDeleteAsync<TEntity>([NotNull] this IRepository<TEntity> repository, [NotNull]Expression<Func<TEntity, bool>> predicate)
            where TEntity : Entity<int>
        {
            return await repository.BatchDeleteAsync<TEntity, int>(predicate);
        }

        /// <summary>
        /// Updates all matching entities using given updateExpression for given predicate
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <typeparam name="TPrimaryKey">Primary key type</typeparam>
        /// <param name="repository">Repository</param>
        /// /// <param name="updateExpression">Update expression</param>
        /// <param name="predicate">Predicate to filter entities</param>
        /// <returns></returns>
        public static async Task<int> BatchUpdateAsync<TEntity, TPrimaryKey>([NotNull]this IRepository<TEntity, TPrimaryKey> repository, [NotNull]Expression<Func<TEntity, TEntity>> updateExpression, [NotNull]Expression<Func<TEntity, bool>> predicate)
            where TEntity : Entity<TPrimaryKey>
        {
            Check.NotNull(repository, nameof(repository));
            Check.NotNull(updateExpression, nameof(updateExpression));
            Check.NotNull(predicate, nameof(predicate));

            var query = repository.GetAll().IgnoreQueryFilters();

            var abpFilterExpression = GetFilterExpressionOrNull<TEntity, TPrimaryKey>(repository.GetIocResolver());
            var filterExpression = ExpressionCombiner.Combine(predicate, abpFilterExpression);

            query = query.Where(filterExpression);

            return await query.UpdateAsync(updateExpression);
        }

        /// <summary>
        /// Updates all matching entities using given updateExpression for given predicate
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <param name="repository">Repository</param>
        /// /// <param name="updateExpression">Update expression</param>
        /// <param name="predicate">Predicate to filter entities</param>
        /// <returns></returns>
        public static async Task<int> BatchUpdateAsync<TEntity>(
            [NotNull]this IRepository<TEntity> repository, [NotNull]Expression<Func<TEntity, TEntity>> updateExpression,
            [NotNull]Expression<Func<TEntity, bool>> predicate)
            where TEntity : Entity<int>
        {
            return await repository.BatchUpdateAsync<TEntity, int>(updateExpression, predicate);
        }

        private static Expression<Func<TEntity, bool>> GetFilterExpressionOrNull<TEntity, TPrimaryKey>(IIocResolver iocResolver) where TEntity : Entity<TPrimaryKey>
        {
            Expression<Func<TEntity, bool>> expression = null;

            using (var scope = iocResolver.CreateScope())
            {
                var currentUnitOfWorkProvider = scope.Resolve<ICurrentUnitOfWorkProvider>();

                if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
                {
                    var isSoftDeleteFilterEnabled = currentUnitOfWorkProvider.Current?.IsFilterEnabled(AbpDataFilters.SoftDelete) == true;
                    if (isSoftDeleteFilterEnabled)
                    {
                        Expression<Func<TEntity, bool>> softDeleteFilter = e => !((ISoftDelete)e).IsDeleted;
                        expression = softDeleteFilter;
                    }
                }

                if (typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity)))
                {
                    var isMayHaveTenantFilterEnabled = currentUnitOfWorkProvider.Current?.IsFilterEnabled(AbpDataFilters.MayHaveTenant) == true;
                    var currentTenantId = GetCurrentTenantIdOrNull(iocResolver);

                    if (isMayHaveTenantFilterEnabled)
                    {
                        Expression<Func<TEntity, bool>> mayHaveTenantFilter = e => ((IMayHaveTenant)e).TenantId == currentTenantId;
                        expression = expression == null ? mayHaveTenantFilter : ExpressionCombiner.Combine(expression, mayHaveTenantFilter);
                    }
                }

                if (typeof(IMustHaveTenant).IsAssignableFrom(typeof(TEntity)))
                {
                    var isMustHaveTenantFilterEnabled = currentUnitOfWorkProvider.Current?.IsFilterEnabled(AbpDataFilters.MustHaveTenant) == true;
                    var currentTenantId = GetCurrentTenantIdOrNull(iocResolver);

                    if (isMustHaveTenantFilterEnabled)
                    {
                        Expression<Func<TEntity, bool>> mustHaveTenantFilter = e => ((IMustHaveTenant)e).TenantId == currentTenantId;
                        expression = expression == null ? mustHaveTenantFilter : ExpressionCombiner.Combine(expression, mustHaveTenantFilter);
                    }
                }
            }

            return expression;
        }

        private static int? GetCurrentTenantIdOrNull(IIocResolver iocResolver)
        {
            using (var scope = iocResolver.CreateScope())
            {
                var currentUnitOfWorkProvider = scope.Resolve<ICurrentUnitOfWorkProvider>();

                if (currentUnitOfWorkProvider?.Current != null)
                {
                    return currentUnitOfWorkProvider.Current.GetTenantId();
                }

                return iocResolver.Resolve<IAbpSession>().TenantId;
            }
        }
    }
}
