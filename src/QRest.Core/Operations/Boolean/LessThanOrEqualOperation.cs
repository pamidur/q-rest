using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public sealed class LessThanOrEqualOperation : CompareOperationBase
    {
        internal LessThanOrEqualOperation() { }

        public override string Key { get; } = "lte";

        protected override Expression PickExpression(Expression a, Expression b)=>
            Expression.LessThanOrEqual(a, b);
    }
}
