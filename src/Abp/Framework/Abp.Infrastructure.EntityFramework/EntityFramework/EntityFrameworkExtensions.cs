using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Abp.EntityFramework
{
    public static class EntityFrameworkExtensions
    {
        private static IQueryable<TEntity> PrivateOrderBy<TEntity>(this IQueryable<TEntity> query, string sorting)
        {
            var parts = sorting.Split(' ');

            var isDescending = false;
            var propertyName = "";
            var tType = typeof(TEntity);

            if (parts.Length > 0 && parts[0] != "")
            {
                propertyName = parts[0];

                if (parts.Length > 1)
                {
                    isDescending = parts[1].ToLower().Contains("asc");
                }

                PropertyInfo prop = tType.GetProperty(propertyName);

                if (prop == null)
                {
                    throw new ArgumentException(string.Format("No property '{0}' on type '{1}'", propertyName, tType.Name));
                }

                var funcType = typeof(Func<,>)
                    .MakeGenericType(tType, prop.PropertyType);

                var lambdaBuilder = typeof(Expression)
                    .GetMethods()
                    .First(x => x.Name == "Lambda" && x.ContainsGenericParameters && x.GetParameters().Length == 2)
                    .MakeGenericMethod(funcType);

                var parameter = Expression.Parameter(tType);
                var propExpress = Expression.Property(parameter, prop);

                var sortLambda = lambdaBuilder
                    .Invoke(null, new object[] { propExpress, new ParameterExpression[] { parameter } });

                var sorter = typeof(Queryable)
                    .GetMethods()
                    .FirstOrDefault(x => x.Name == (isDescending ? "OrderByDescending" : "OrderBy") && x.GetParameters().Length == 2)
                    .MakeGenericMethod(new[] { tType, prop.PropertyType });

                return (IQueryable<TEntity>)sorter.Invoke(null, new object[] { query, sortLambda });
            }
            return query;
        }

        private static IQueryable<TEntity> PrivateThenBy<TEntity>(this IQueryable<TEntity> query, string sorting)
        {
            var parts = sorting.Split(' ');

            var isDescending = false;
            var propertyName = "";
            var tType = typeof(TEntity);

            if (parts.Length > 0 && parts[0] != "")
            {
                propertyName = parts[0];

                if (parts.Length > 1)
                {
                    isDescending = parts[1].ToLower().Contains("asc");
                }

                PropertyInfo prop = tType.GetProperty(propertyName);

                if (prop == null)
                {
                    throw new ArgumentException(string.Format("No property '{0}' on type '{1}'", propertyName, tType.Name));
                }

                var funcType = typeof(Func<,>)
                    .MakeGenericType(tType, prop.PropertyType);

                var lambdaBuilder = typeof(Expression)
                    .GetMethods()
                    .First(x => x.Name == "Lambda" && x.ContainsGenericParameters && x.GetParameters().Length == 2)
                    .MakeGenericMethod(funcType);

                var parameter = Expression.Parameter(tType);
                var propExpress = Expression.Property(parameter, prop);

                var sortLambda = lambdaBuilder
                    .Invoke(null, new object[] { propExpress, new ParameterExpression[] { parameter } });

                var sorter = typeof(Queryable)
                    .GetMethods()
                    .FirstOrDefault(x => x.Name == (isDescending ? "ThenByDescending" : "ThenBy") && x.GetParameters().Length == 2)
                    .MakeGenericMethod(new[] { tType, prop.PropertyType });

                return (IQueryable<TEntity>)sorter.Invoke(null, new object[] { query, sortLambda });
            }
            return query;
        }

        public static IQueryable<TEntity> SortedBy<TEntity>(this IQueryable<TEntity> query, string sorting)
        {
            //
            if (query == null)
                throw new ArgumentNullException("source", "source is null.");

            if (string.IsNullOrEmpty(sorting))
                throw new ArgumentException("sortExpression is null or empty.", "sortExpression");

            //sacamos las parte que contengan una coma
            var ordesPropierties = sorting.Split(',');
            if (ordesPropierties.Length > 0 && ordesPropierties[0] != "")
            {

                query = query.PrivateOrderBy(ordesPropierties[0]);
                for (int i = 1; i < ordesPropierties.Length; i++)
                    query = query.PrivateThenBy(ordesPropierties[i]);

            }

            return query;
        }

        public static IQueryable<TEntity> PagedBy<TEntity>(this IQueryable<TEntity> query, int skipCount, int maxResultCount)
        {
            if (query == null)
                throw new ArgumentNullException("source", "source is null.");
            return query.Skip(skipCount).Take(maxResultCount);
        }
    }
}
