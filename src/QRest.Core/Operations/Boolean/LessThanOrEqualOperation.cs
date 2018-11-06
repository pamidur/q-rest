using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public class LessThanOrEqualOperation : CompareOperationBase
    {
        public override string Key { get; } = "lte";

        protected override Expression PickExpression(Expression a, Expression b)=>
            Expression.LessThanOrEqual(a, b);
    }
}
