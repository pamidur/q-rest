using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public sealed class NotEqualOperation : CompareOperationBase
    {
        internal NotEqualOperation() { }

        public override string Key { get; } = "ne";

        protected override Expression PickExpression(Expression a, Expression b) =>
            Expression.NotEqual(a, b);
    }
}

