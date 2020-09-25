using System;
using System.Linq.Expressions;
using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Uow;
using Abp.Linq.Expressions;
using Abp.Runtime.Session;

namespace Abp.EntityFrameworkCore
{
    public static class AbpEntityFrameworkCoreExtension
    {
        internal static Expression<Func<TEntity, bool>> GetFilterExpressionOrNull<TEntity, TPrimaryKey>(IIocResolver iocResolver)
            where TEntity : IEntity<TPrimaryKey>
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

                if (typeof(IMayHaveBranch).IsAssignableFrom(typeof(TEntity)))
                {
                    var isMayHaveBranchFilterEnabled = currentUnitOfWorkProvider.Current?.IsFilterEnabled(AbpDataFilters.MayHaveBranch) == true;
                    var currentBranchId = GetCurrentBranchIdOrNull(iocResolver);

                    if (isMayHaveBranchFilterEnabled)
                    {
                        Expression<Func<TEntity, bool>> mayHaveBranchFilter = e => currentBranchId == null || ((IMayHaveBranch)e).BranchId == currentBranchId;
                        expression = expression == null ? mayHaveBranchFilter : ExpressionCombiner.Combine(expression, mayHaveBranchFilter);
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

                if (typeof(IMustHaveBranch).IsAssignableFrom(typeof(TEntity)))
                {
                    var isMustHaveBranchFilterEnabled = currentUnitOfWorkProvider.Current?.IsFilterEnabled(AbpDataFilters.MustHaveBranch) == true;
                    var currentBranchId = GetCurrentBranchIdOrNull(iocResolver);

                    if (isMustHaveBranchFilterEnabled)
                    {
                        Expression<Func<TEntity, bool>> mustHaveBranchFilter = e => currentBranchId == null || ((IMustHaveBranch)e).BranchId == currentBranchId;
                        expression = expression == null ? mustHaveBranchFilter : ExpressionCombiner.Combine(expression, mustHaveBranchFilter);
                    }
                }
            }

            return expression;
        }

        private static long? GetCurrentTenantIdOrNull(IIocResolver iocResolver)
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
        private static long? GetCurrentBranchIdOrNull(IIocResolver iocResolver)
        {
            using (var scope = iocResolver.CreateScope())
            {
                var currentUnitOfWorkProvider = scope.Resolve<ICurrentUnitOfWorkProvider>();

                if (currentUnitOfWorkProvider?.Current != null)
                {
                    return currentUnitOfWorkProvider.Current.GetBranchId();
                }

                return iocResolver.Resolve<IAbpSession>().BranchId;
            }
        }
    }
}
