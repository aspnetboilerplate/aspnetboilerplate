using System;
using System.Linq.Expressions;

namespace Abp.Specifications
{
    /// <summary>
    /// Any specification ,raw is true specification
    /// </summary>
    /// <typeparam name="TEntity">Type of entity in this specification</typeparam>
    public sealed class AnySpecification<TEntity>
        : Specification<TEntity>
        where TEntity : class
    {
        #region Specification overrides

        /// <summary>
        /// <see cref="Specification{TEntity}"/>
        /// </summary>
        /// <returns><see cref="Specification{TEntity}"/></returns>
        public override System.Linq.Expressions.Expression<Func<TEntity, bool>> SatisfiedBy()
        {
            //Create "result variable" transform adhoc execution plan in prepared plan
            //for more info: http://geeks.ms/blogs/unai/2010/07/91/ef-4-0-performance-tips-1.aspx
            bool result = true;

            Expression<Func<TEntity, bool>> trueExpression = t => result;
            return trueExpression;
        }

        #endregion
    }
}
