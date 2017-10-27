using System.Collections.Generic;
using System.Linq.Expressions;

namespace QueryableRest.Semantics.Operations
{
    public class NotOperation : IOperation
    {
        public static readonly string DefaultMoniker = "not";

        public Expression CreateExpression(Expression context, Expression argumentsRoot, IReadOnlyList<Expression> arguments)
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
