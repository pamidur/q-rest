using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public class NotEqualOperation : CompareOperationBase
    {
        protected override Expression PickExpression(Expression a, Expression b) =>
            Expression.NotEqual(a, b);
    }
}
