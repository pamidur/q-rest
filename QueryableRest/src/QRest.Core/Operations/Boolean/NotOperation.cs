using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public class NotOperation : OperationBase
    {
        public override bool SupportsCall => true;

        public override Expression CreateCallExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count != 0)
                throw new ExpressionCreationException();

            return Expression.Not(context);
        }
    }
}
