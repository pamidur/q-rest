using QRest.Core.Extensions;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public abstract class TermBase : ITerm
    {
        public ITerm Next { get; set; }

        public Expression CreateExpressionChain(Expression prev, ParameterExpression root)
        {
            var exp = CreateExpression(prev, root);
            return Next?.CreateExpressionChain(exp, root) ?? Finalize(exp);
        }

        private Expression Finalize(Expression exp)
        {
            var eType = exp.GetQueryElementType();
            //todo:: get prev name
            exp = eType == null ? exp :
                Expression.Call(typeof(Enumerable), nameof(Enumerable.ToList), new[] { eType }, exp);

            return exp;
        }

        protected abstract Expression CreateExpression(Expression prev, ParameterExpression root);

        public sealed override string ToString()
        {
            return $"{Debug}{Next?.ToString()}";
        }

        protected virtual string Debug => $"#{GetType().Name.ToLowerInvariant().Replace("term", "")}";
    }
}
