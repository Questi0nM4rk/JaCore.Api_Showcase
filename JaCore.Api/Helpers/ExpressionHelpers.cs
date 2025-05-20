using System.Linq.Expressions;

namespace JaCore.Api.Helpers
{
    public static class ExpressionHelpers
    {
        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> left, Expression<Func<T, bool>> right)
        {
            var rightVisited = new ExpressionParameterReplacer(right.Parameters[0], left.Parameters[0]).Visit(right.Body);
            if (rightVisited == null) throw new InvalidOperationException("Could not replace parameter in expression.");
            
            var andAlso = Expression.AndAlso(left.Body, rightVisited);
            return Expression.Lambda<Func<T, bool>>(andAlso, left.Parameters);
        }
    }

    internal class ExpressionParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _source;
        private readonly ParameterExpression _target;

        public ExpressionParameterReplacer(ParameterExpression source, ParameterExpression target)
        {
            _source = source;
            _target = target;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _source ? _target : base.VisitParameter(node);
        }
    }
} 