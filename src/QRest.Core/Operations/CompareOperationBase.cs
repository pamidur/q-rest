using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public abstract class CompareOperationBase : OperationBase
    {
        public override bool SupportsCall => true;        

        public override Expression CreateCallExpression(Expression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            var compareArgs = Convert(context, arguments[0]);

            return PickExpression(compareArgs.Left, compareArgs.Right);
        }

        protected abstract Expression PickExpression(Expression a, Expression b);

        protected virtual (Expression Left, Expression Right) Convert(Expression left, Expression right)
        {
            var leftType = /*left.NodeType == ExpressionType.Lambda ? ((LambdaExpression)left).ReturnType : */left.Type;
            var rightType = /*right.NodeType == ExpressionType.Lambda ? ((LambdaExpression)right).ReturnType :*/ right.Type;

            if (TryCast(right, leftType, out var newright))
                return (left, newright);
            else if (TryCast(left, rightType, out var newleft))
                return (newleft, right);
            else
                throw new ExpressionCreationException($"Cannot compare {leftType.Name} and {rightType.Name}");
        }       
    }
}
