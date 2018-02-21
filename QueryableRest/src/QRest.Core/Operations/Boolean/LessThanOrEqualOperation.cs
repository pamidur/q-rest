using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public class LessThanOrEqualOperation : CompareOperationBase
    {
        protected override Expression PickExpression(Expression a, Expression b)=>
            Expression.LessThanOrEqual(a, b);
    }
}
