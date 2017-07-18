using System;
using System.Linq.Expressions;

namespace Abp.Dapper.Utils
{
    internal class ExpressionUtils
    {
        /// <summary>
        ///     Makes the predicate.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="typeOfValue">The type of value.</param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> MakePredicate<T>(
            string name,
            object value,
            Type typeOfValue = null)
        {
            ParameterExpression param = Expression.Parameter(typeof(T), typeof(T).Name);
            MemberExpression memberExp = Expression.Property(param, name);
            BinaryExpression body = Expression.Equal(memberExp, typeOfValue == null ? Expression.Constant(value) : Expression.Constant(value, typeOfValue));
            return Expression.Lambda<Func<T, bool>>(body, param);
        }
    }
}
