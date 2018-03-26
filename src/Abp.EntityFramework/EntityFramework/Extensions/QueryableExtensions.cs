using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

using Abp.Dependency;
using Abp.Domain.Entities;
using Abp.Domain.Repositories;
using Abp.EntityFramework.Interceptors;
using Abp.Extensions;

using JetBrains.Annotations;

namespace Abp.EntityFramework.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IQueryable"/> and <see cref="IQueryable{T}"/>.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="source">The source <see cref="IQueryable"/> on which to call Include.</param>
        /// <param name="condition">A boolean value to determine to include <paramref name="path"/> or not.</param>
        /// <param name="path">The dot-separated list of related objects to return in the query results.</param>
        public static IQueryable IncludeIf(this IQueryable source, bool condition, string path)
        {
            return condition
                ? source.Include(path)
                : source;
        }

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="source">The source <see cref="IQueryable{T}"/> on which to call Include.</param>
        /// <param name="condition">A boolean value to determine to include <paramref name="path"/> or not.</param>
        /// <param name="path">The dot-separated list of related objects to return in the query results.</param>
        public static IQueryable<T> IncludeIf<T>(this IQueryable<T> source, bool condition, string path)
        {
            return condition
                ? source.Include(path)
                : source;
        }

        /// <summary>
        /// Specifies the related objects to include in the query results.
        /// </summary>
        /// <param name="source">The source <see cref="IQueryable{T}"/> on which to call Include.</param>
        /// <param name="condition">A boolean value to determine to include <paramref name="path"/> or not.</param>
        /// <param name="path">The type of navigation property being included.</param>
        public static IQueryable<T> IncludeIf<T, TProperty>(this IQueryable<T> source, bool condition, Expression<Func<T, TProperty>> path)
        {
            return condition
                ? source.Include(path)
                : source;
        }

        /// <summary>
        /// Nolockings the specified queryable.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="queryable">The queryable.</param>
        /// <returns></returns>
        public static TResult Nolocking<TEntity, TResult>(this IRepository<TEntity, int> repository, [NotNull] Func<IQueryable<TEntity>, TResult> queryable) where TEntity : class, IEntity<int>
        {
            Check.NotNull(queryable, nameof(queryable));

            TResult result;

            using (var nolockInterceptor = repository.As<AbpRepositoryBase<TEntity, int>>().IocResolver.ResolveAsDisposable<WithNoLockInterceptor>())
            {
                using (nolockInterceptor.Object.UseNolocking())
                {
                    result = queryable(repository.GetAll());
                }
            }

            return result;
        }

        /// <summary>
        /// Nolockings the specified queryable.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity.</typeparam>
        /// <typeparam name="TPrimaryKey">The type of the primary key.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="repository">The repository.</param>
        /// <param name="queryable">The queryable.</param>
        /// <returns></returns>
        public static TResult Nolocking<TEntity, TPrimaryKey, TResult>(this IRepository<TEntity, TPrimaryKey> repository, [NotNull] Func<IQueryable<TEntity>, TResult> queryable) where TEntity : class, IEntity<TPrimaryKey>
        {
            Check.NotNull(queryable, nameof(queryable));

            TResult result;

            using (var nolockInterceptor = repository.As<AbpRepositoryBase<TEntity, TPrimaryKey>>().IocResolver.ResolveAsDisposable<WithNoLockInterceptor>())
            {
                using (nolockInterceptor.Object.UseNolocking())
                {
                    result = queryable(repository.GetAll());
                }
            }

            return result;
        }
    }
}