using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using DapperExtensions;
using JetBrains.Annotations;

namespace Abp.Dapper.Extensions
{
    internal static class SortingExtensions
    {
        [NotNull]
        public static List<ISort> ToSortable<T>([NotNull] this Expression<Func<T, object>>[] sortingExpression, bool ascending = true)
        {
            Check.NotNullOrEmpty(sortingExpression, nameof(sortingExpression));

            var sortList = new List<ISort>();
            sortingExpression.ToList().ForEach(sortExpression =>
            {
                MemberInfo sortProperty = ReflectionHelper.GetProperty(sortExpression);
                sortList.Add(new Sort { Ascending = ascending, PropertyName = sortProperty.Name });
            });

            return sortList;
        }
    }
}
