using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Runtime.Session;
using Microsoft.EntityFrameworkCore;
using Z.EntityFramework.Plus;

namespace Abp.EntityFrameworkCore.EFPlus
{
    public static class AbpEntityFrameworkCoreEfPlusExtensions
    {
        public static async Task<int> BatchDeleteAllAsync<TEntity>(this IRepository<TEntity> repository) where TEntity : Entity<int>
        {
            return await repository.BatchDeleteAsync(null);
        }

        public static async Task<int> BatchDeleteAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate) where TEntity : Entity<int>
        {
            var query = repository.GetAll().IgnoreQueryFilters();

            var abpFilterExpression = GetFilterExpressionOrNull(query, repository.GetIocResolver());
            var filterExpression = CombineExpressions(predicate, abpFilterExpression);
            
            if (filterExpression != null)
            {
                query = query.Where(filterExpression);
            }

            return await query.DeleteAsync();
        }

        public static async Task<int> BatchUpdateAllAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, TEntity>> updateExpression) where TEntity : Entity<int>
        {
            return await repository.BatchUpdateAsync(updateExpression, null);
        }

        public static async Task<int> BatchUpdateAsync<TEntity>(this IRepository<TEntity> repository, Expression<Func<TEntity, TEntity>> updateExpression, Expression<Func<TEntity, bool>> predicate) where TEntity : Entity<int>
        {
            var query = repository.GetAll().IgnoreQueryFilters();

            var abpFilterExpression = GetFilterExpressionOrNull(query, repository.GetIocResolver());
            var filterExpression = CombineExpressions(predicate, abpFilterExpression);
            
            if (filterExpression != null)
            {
                query = query.Where(filterExpression);
            }
            
            return await query.UpdateAsync(updateExpression);
        }

        private static Expression<Func<TEntity, bool>> GetFilterExpressionOrNull<TEntity>(IQueryable<TEntity> query, IIocResolver iocResolver) where TEntity : Entity<int>
        {
            Expression<Func<TEntity, bool>> expression = null;

            if (typeof(ISoftDelete).IsAssignableFrom(typeof(TEntity)))
            {
                var isSoftDeleteFilterEnabled = iocResolver.Resolve<ICurrentUnitOfWorkProvider>().Current?.IsFilterEnabled(AbpDataFilters.SoftDelete) == true;
                if (isSoftDeleteFilterEnabled)
                {
                    Expression<Func<TEntity, bool>> softDeleteFilter = e => !((ISoftDelete)e).IsDeleted;
                    expression = expression == null ? softDeleteFilter : CombineExpressions(expression, softDeleteFilter);
                }
            }

            if (typeof(IMayHaveTenant).IsAssignableFrom(typeof(TEntity)))
            {
                var isMayHaveTenantFilterEnabled = iocResolver.Resolve<ICurrentUnitOfWorkProvider>().Current?.IsFilterEnabled(AbpDataFilters.MayHaveTenant) == true;
                var currentTenantId = GetCurrentTenantIdOrNull(iocResolver);

                if (isMayHaveTenantFilterEnabled)
                {
                    Expression<Func<TEntity, bool>> mayHaveTenantFilter = e => ((IMayHaveTenant)e).TenantId == currentTenantId;
                    expression = expression == null ? mayHaveTenantFilter : CombineExpressions(expression, mayHaveTenantFilter);
                }
            }

            if (typeof(IMustHaveTenant).IsAssignableFrom(typeof(TEntity)))
            {
                var isMustHaveTenantFilterEnabled = iocResolver.Resolve<ICurrentUnitOfWorkProvider>().Current?.IsFilterEnabled(AbpDataFilters.MustHaveTenant) == true;
                var currentTenantId = GetCurrentTenantIdOrNull(iocResolver);

                if (isMustHaveTenantFilterEnabled)
                {
                    Expression<Func<TEntity, bool>> mustHaveTenantFilter = e => ((IMustHaveTenant)e).TenantId == currentTenantId;
                    expression = expression == null ? mustHaveTenantFilter : CombineExpressions(expression, mustHaveTenantFilter);
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

        private static Expression<Func<T, bool>> CombineExpressions<T>(Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            if (expression1 == null && expression2 == null)
            {
                return null;
            }

            if (expression1 == null)
            {
                return expression2;
            }

            if (expression2 == null)
            {
                return expression1;
            }

            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceExpressionVisitor(expression1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expression1.Body);

            var rightVisitor = new ReplaceExpressionVisitor(expression2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expression2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left, right), parameter);
        }

        class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression Visit(Expression node)
            {
                if (node == _oldValue)
                {
                    return _newValue;
                }

                return base.Visit(node);
            }
        }
    }
}
