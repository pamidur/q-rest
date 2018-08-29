using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public class GreaterThanOrEqualOperation : CompareOperationBase
    {
        protected override Expression PickExpression(Expression a, Expression b) =>
            Expression.GreaterThanOrEqual(a, b);
    }
}
