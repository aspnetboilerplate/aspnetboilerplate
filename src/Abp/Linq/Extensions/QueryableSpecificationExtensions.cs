using System.Linq;
using Abp.Specifications;

namespace Abp.Linq.Extensions
{
    public static class QueryableSpecificationExtensions
    {
        //TODO: Create other extension method for the IQueryable that takes Specification...?

        public static IQueryable<T> Where<T>(this IQueryable<T> source, ISpecification<T> specification)
        {
            return source.Where(specification.ToExpression());
        }
    }
}