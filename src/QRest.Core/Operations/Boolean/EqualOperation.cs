using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public class EqualOperation : CompareOperationBase
    {
        public override string Key { get; } = "eq";

        protected override Expression PickExpression(Expression a, Expression b) =>
            Expression.Equal(a, b);
    }
}
