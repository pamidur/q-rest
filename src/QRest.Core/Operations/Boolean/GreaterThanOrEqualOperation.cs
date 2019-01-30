using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public sealed class GreaterThanOrEqualOperation : CompareOperationBase
    {
        internal GreaterThanOrEqualOperation() { }

        public override string Key { get; } = "gte";

        protected override Expression PickExpression(Expression a, Expression b) =>
            Expression.GreaterThanOrEqual(a, b);
    }
}
