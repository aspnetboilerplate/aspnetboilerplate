using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Abp.Dapper.Expressions
{
    /// <summary>
    ///     Reference
    ///     From:http://blogs.msdn.com/b/mattwar/archive/2007/08/01/linq-building-an-iqueryable-provider-part-iii.aspx
    /// </summary>
    internal class Evaluator
    {
        public static Expression PartialEval(Expression exp, Func<Expression, bool> canBeEval)
        {
            return new SubtreeEvaluator(new Nominator(canBeEval).Nominate(exp)).Eval(exp);
        }

        public static Expression PartialEval(Expression exp)
        {
            return PartialEval(exp, CanBeEvaluatedLocally);
        }

        private static bool CanBeEvaluatedLocally(Expression exp)
        {
            return exp.NodeType != ExpressionType.Parameter;
        }

        private class SubtreeEvaluator : ExpressionVisitor
        {
            private readonly HashSet<Expression> _candidates;

            internal SubtreeEvaluator(HashSet<Expression> candidates)
            {
                _candidates = candidates;
            }

            internal Expression Eval(Expression exp)
            {
                return Visit(exp);
            }

            public override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }

                if (_candidates.Contains(exp))
                {
                    return Evaluate(exp);
                }

                return base.Visit(exp);
            }

            private Expression Evaluate(Expression exp)
            {
                if (exp.NodeType == ExpressionType.Constant)
                {
                    return exp;
                }

                LambdaExpression lambda = Expression.Lambda(exp);
                Delegate del = lambda.Compile();

                return Expression.Constant(del.DynamicInvoke(null), exp.Type);
            }
        }

        private class Nominator : ExpressionVisitor
        {
            private readonly Func<Expression, bool> _canBeEval;
            private HashSet<Expression> _candidates;
            private bool _cannotBeEval;

            internal Nominator(Func<Expression, bool> canBeEval)
            {
                _canBeEval = canBeEval;
            }

            internal HashSet<Expression> Nominate(Expression exp)
            {
                _candidates = new HashSet<Expression>();
                Visit(exp);
                return _candidates;
            }

            public override Expression Visit(Expression exp)
            {
                if (exp == null)
                {
                    return null;
                }

                bool saveCannotBeEval = _cannotBeEval;
                _cannotBeEval = false;

                base.Visit(exp);

                if (!_cannotBeEval)
                {
                    if (_canBeEval(exp))
                    {
                        _candidates.Add(exp);
                    }
                    else
                    {
                        _cannotBeEval = true;
                    }
                }

                _cannotBeEval |= saveCannotBeEval;

                return exp;
            }
        }
    }
}
