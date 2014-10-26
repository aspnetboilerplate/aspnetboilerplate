using System;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Abp.Application.Services.Dto;

namespace Abp.Linq.Extensions
{
    /// <summary>
    /// Some useful extension methods for IQueryable.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Used for paging. Can be used as an alternative to Skip(...).Take(...) chaining.
        /// </summary>
        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, int skipCount, int maxResultCount)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            return query.Skip(skipCount).Take(maxResultCount);
        }

        /// <summary>
        /// Used for paging with an <see cref="IPagedResultRequest"/> object.
        /// </summary>
        /// <param name="query">Queryable to apply paging</param>
        /// <param name="pagedResultRequest">An object implements <see cref="IPagedResultRequest"/> interface</param>
        /// <returns></returns>
        public static IQueryable<T> PageBy<T>(this IQueryable<T> query, IPagedResultRequest pagedResultRequest)
        {
            return query.PageBy(pagedResultRequest.SkipCount, pagedResultRequest.MaxResultCount);
        }

        /// <summary>
        /// Used to order given queryable by multiple sorting directions.
        /// Some sorting samples:
        /// "Name"
        /// "Name ASC"
        /// "Name, Age DESC"
        /// "Name ASC,Age"
        /// ASC and DESC are optional and case-insensitive.
        /// Adjacent spaces of commas (,) are ignored.
        /// sorting can be null or empty string. In this case, query is not ordered.
        /// </summary>
        public static IQueryable<T> MultipleOrderBy<T>(this IQueryable<T> query, string sorting)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            if (string.IsNullOrWhiteSpace(sorting))
            {
                return query;
            }

            var orderingProps = sorting.Split(',').Select(s => s.Trim()).ToList();

            query = query.SingleOrderBy(orderingProps[0], "OrderBy");

            for (var i = 1; i < orderingProps.Count; i++)
            {
                query = query.SingleOrderBy(orderingProps[i], "ThenBy");
            }

            return query;
        }

        #region Private methods

        private static IQueryable<T> SingleOrderBy<T>(this IQueryable<T> query, string sorting, string orderMethodPrefix)
        {
            var itemType = typeof(T);
            var sortingParts = sorting.Split(' ');

            var sortingPropName = sortingParts[0];
            var sortingProp = itemType.GetProperty(sortingPropName);
            if (sortingProp == null)
            {
                throw new ArgumentException(string.Format("No property '{0}' on type '{1}'", sortingPropName, itemType.Name));
            }

            var isDescending = (sortingParts.Length > 1) && sortingParts[1].ToUpper(CultureInfo.InvariantCulture).EndsWith("DESC");

            var parameter = Expression.Parameter(itemType);
            var propExpress = Expression.Property(parameter, sortingProp);
            var lambdaBuilder = typeof(Expression)
                .GetMethods()
                .First(x => x.Name == "Lambda" && x.ContainsGenericParameters && x.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(Func<,>).MakeGenericType(itemType, sortingProp.PropertyType));

            var sortLambda = lambdaBuilder
                .Invoke(null, new object[] { propExpress, new[] { parameter } });

            var sorter = typeof(Queryable)
                .GetMethods()
                .FirstOrDefault(x => x.Name == (orderMethodPrefix + (isDescending ? "Descending" : "")) && x.GetParameters().Length == 2)
                .MakeGenericMethod(new[] { itemType, sortingProp.PropertyType });

            return (IQueryable<T>)sorter.Invoke(null, new[] { query, sortLambda });
        }

        #endregion
    }
}
