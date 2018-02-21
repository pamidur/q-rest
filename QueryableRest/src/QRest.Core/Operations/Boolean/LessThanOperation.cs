using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public class LessThanOperation : CompareOperationBase
    {
        protected override Expression PickExpression(Expression a, Expression b)=>
            Expression.LessThan(a, b);
    }
}
