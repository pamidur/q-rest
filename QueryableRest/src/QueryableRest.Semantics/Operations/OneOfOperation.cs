using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QueryableRest.Semantics.Operations
{
    public class OneOfOperation :IOperation
    {
        public static readonly string DefaultMoniker = "oneof";

        public Expression CreateExpression(Expression context, Expression argumentsRoot, IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count < 1)
                throw new ExpressionCreationException();

            var body = arguments.Aggregate((e1, e2) => { return Expression.OrElse(e1, e2); });

            return body;
        }

        public Expression GetArgumentsRoot(Expression context)
        {
            return context;
        }
    }
}
