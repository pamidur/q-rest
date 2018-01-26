using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public class NotOperation : IOperation
    {
        public Expression CreateExpression(Expression last, ParameterExpression root, IReadOnlyList<Expression> arguments, QueryContext context)
        {
            if (arguments.Count != 0)
                throw new ExpressionCreationException();            

            return Expression.Not(last);
        }

        public Expression GetArgumentsRoot(Expression context)
        {
            return context;
        }
    }
}
