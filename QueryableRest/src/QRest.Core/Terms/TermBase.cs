using System.Linq.Expressions;

namespace QRest.Core.Terms
{
    public abstract class TermBase : ITerm
    {
        public ITerm Next { get; set; }

        public Expression CreateExpressionChain(Expression prev, ParameterExpression root)
        {
            var exp = CreateExpression(prev, root);
            return Next?.CreateExpressionChain(exp, root) ?? exp;
        }

        protected abstract Expression CreateExpression(Expression prev, ParameterExpression root);

        public sealed override string ToString()
        {
            return $"{Debug}{Next?.ToString()}";
        }

        protected virtual string Debug => $"#{GetType().Name.ToLowerInvariant().Replace("term", "")}";
    }
}
