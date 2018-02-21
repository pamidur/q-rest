using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public class NotEqualOperation : CompareOperationBase
    {
        public override bool SupportsCall => true;

        protected override Expression PickExpression(Expression a, Expression b)
        {
            return Expression.NotEqual(a, b);
        }
    }
}
