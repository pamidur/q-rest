using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public class NotOperation : IOperation
    {
        public static readonly string DefaultMoniker = "not";

        public Expression CreateExpression(Expression context, ParameterExpression root, IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count != 0)
                throw new ExpressionCreationException();            

            return Expression.Not(context);
        }

        public Expression GetArgumentsRoot(Expression context)
        {
            return context;
        }
    }
}
