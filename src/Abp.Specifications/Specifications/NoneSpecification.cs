namespace Abp.Specifications
{
    using System;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents the specification that can be satisfied by the given object
    /// in no circumstance.
    /// </summary>
    /// <typeparam name="T">The type of the object to which the specification is applied.</typeparam>
    public sealed class NoneSpecification<T> : Specification<T>
    {
        #region Public Methods
        /// <summary>
        /// Gets the LINQ expression which represents the current specification.
        /// </summary>
        /// <returns>The LINQ expression.</returns>
        public override Expression<Func<T, bool>> GetExpression()
        {
            return o => false;
        }
        #endregion
    }
}
