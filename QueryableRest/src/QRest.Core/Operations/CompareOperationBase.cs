using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public abstract class CompareOperationBase : OperationBase
    {
        public override bool SupportsCall => true;        

        public override Expression CreateCallExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            var compareArgs = Convert(context, arguments[0]);

            return PickExpression(compareArgs.Left, compareArgs.Right);
        }

        protected abstract Expression PickExpression(Expression a, Expression b);

        protected virtual (Expression Left, Expression Right) Convert(Expression left, Expression right)
        {
            if (TryCast(right, left.Type, out var newright))
                return (left, newright);
            else if (TryCast(left, right.Type, out var newleft))
                return (newleft, right);
            else
                throw new ExpressionCreationException($"Cannot compare {left.Type.Name} and {right.Type.Name}");
        }       
    }
}
