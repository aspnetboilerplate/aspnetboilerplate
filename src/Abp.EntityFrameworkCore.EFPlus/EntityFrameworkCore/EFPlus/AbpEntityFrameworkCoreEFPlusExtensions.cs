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
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Abp.EntityFrameworkCore.EFPlus
{
    /// <summary>
    /// 
    /// </summary>
    public static class AbpEntityFrameworkCoreEfPlusExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static async Task<int> BatchDeleteAllAsync<TEntity>(this IRepository<TEntity> repository) where TEntity : Entity<int>
        {
            return await repository.BatchDeleteAsync(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static async Task<int> BatchDeleteAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate) where TEntity : Entity<int>
        {
            var query = repository.GetAll().IgnoreQueryFilters();

            var abpFilterExpression = GetFilterExpressionOrNull<TEntity>(repository.GetIocResolver());
            var filterExpression = ExpressionCombiner.Combine(predicate, abpFilterExpression);

            if (filterExpression != null)
            {
                query = query.Where(filterExpression);
            }

            return await query.DeleteAsync();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="updateExpression"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static async Task<int> BatchUpdateAllAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, TEntity>> updateExpression) where TEntity : Entity<int>
        {
            return await repository.BatchUpdateAsync(updateExpression, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="updateExpression"></param>
        /// <param name="predicate"></param>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public static async Task<int> BatchUpdateAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> predicate) where TEntity : Entity<int>
        {
            var query = repository.GetAll().IgnoreQueryFilters();

            var abpFilterExpression = GetFilterExpressionOrNull<TEntity>(repository.GetIocResolver());
            var filterExpression = ExpressionCombiner.Combine(predicate, abpFilterExpression);

            if (filterExpression != null)
            {
                query = query.Where(filterExpression);
            }

            return await query.UpdateAsync(updateExpression);
        }

        private static Expression<Func<TEntity, bool>> GetFilterExpressionOrNull<TEntity>(IIocResolver iocResolver) where TEntity : Entity<int>
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                var isSoftDeleteFilterEnabled = iocResolver.Resolve<ICurrentUnitOfWorkProvider>().Current?.IsFilterEnabled(AbpDataFilters.SoftDelete) == true;
                if (isSoftDeleteFilterEnabled)
                {
                    Expression<Func<TEntity, bool>> softDeleteFilter = e => !((ISoftDelete)e).IsDeleted;
                    expression = softDeleteFilter;
                }
            }

            if (typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity)))
            {
                var isMayHaveTenantFilterEnabled = iocResolver.Resolve<ICurrentUnitOfWorkProvider>().Current?.IsFilterEnabled(AbpDataFilters.MayHaveTenant) == true;
                var currentTenantId = GetCurrentTenantIdOrNull(iocResolver);

                if (isMayHaveTenantFilterEnabled)
                {
                    Expression<Func<TEntity, bool>> mayHaveTenantFilter = e => ((IMayHaveTenant)e).TenantId == currentTenantId;
                    expression = expression == null ? mayHaveTenantFilter : ExpressionCombiner.Combine(expression, mayHaveTenantFilter);
                }
            }

            if (typeof(IMustHaveTenant).IsAssignableFrom(typeof(TEntity)))
            {
                var isMustHaveTenantFilterEnabled = iocResolver.Resolve<ICurrentUnitOfWorkProvider>().Current?.IsFilterEnabled(AbpDataFilters.MustHaveTenant) == true;
                var currentTenantId = GetCurrentTenantIdOrNull(iocResolver);

                if (isMustHaveTenantFilterEnabled)
                {
                    Expression<Func<TEntity, bool>> mustHaveTenantFilter = e => ((IMustHaveTenant)e).TenantId == currentTenantId;
                    expression = expression == null ? mustHaveTenantFilter : ExpressionCombiner.Combine(expression, mustHaveTenantFilter);
                }
            }

            return expression;
        }

        private static int? GetCurrentTenantIdOrNull(IIocResolver iocResolver)
        {
            var currentUnitOfWorkProvider = iocResolver.Resolve<ICurrentUnitOfWorkProvider>();

            if (currentUnitOfWorkProvider != null &&
                currentUnitOfWorkProvider.Current != null)
            {
                return currentUnitOfWorkProvider.Current.GetTenantId();
            }

            return iocResolver.Resolve<IAbpSession>().TenantId;
        }
    }
}
