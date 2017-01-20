namespace Abp.Specifications
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents the parameter rebinder used for rebinding the parameters
    /// for the given expressions. This is part of the solution which solves
    /// the expression parameter problem when going to Entity Framework by using
    /// Apworks specifications. For more information about this solution please
    /// refer to http://blogs.msdn.com/b/meek/archive/2008/05/02/linq-to-entities-combining-predicates.aspx.
    /// </summary>
    internal class ParameterRebinder : ExpressionVisitor
    {
        #region Private Fields
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;
        #endregion

        #region Ctor
        internal ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }
        #endregion

        #region Internal Static Methods
        internal static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }
        #endregion

        #region Protected Methods
        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (this.map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
        #endregion
    }
}
