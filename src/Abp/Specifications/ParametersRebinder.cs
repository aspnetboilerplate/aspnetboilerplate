using System.Collections.Generic;
using System.Linq.Expressions;

namespace Abp.Specifications
{
    /// <summary>
    /// Helper for rebinder parameters without use Invoke method in expressions 
    /// ( this methods is not supported in all linq query providers, 
    /// for example in Linq2Entities is not supported)
    /// For more information about this solution please refer to http://blogs.msdn.com/b/meek/archive/2008/05/02/linq-to-entities-combining-predicates.aspx.
    /// </summary>
    internal class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> _map;

        /// <summary>
        /// Default construcotr
        /// </summary>
        /// <param name="map">Map specification</param>
        internal ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            _map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }

        /// <summary>
        /// Replate parameters in expression with a Map information
        /// </summary>
        /// <param name="map">Map information</param>
        /// <param name="exp">Expression to replace parameters</param>
        /// <returns>Expression with parameters replaced</returns>
        internal static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }

        /// <summary>
        /// Visit pattern method
        /// </summary>
        /// <param name="p">A Parameter expression</param>
        /// <returns>New visited expression</returns>
        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (_map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }

            return base.VisitParameter(p);
        }
    }
}
