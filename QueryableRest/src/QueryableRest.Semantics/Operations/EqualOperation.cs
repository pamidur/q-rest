using System.Collections.Generic;
using System.Linq.Expressions;

namespace QueryableRest.Semantics.Operations
{
    public class EqualOperation : IOperation
    {
        public static readonly string DefaultMoniker = "eq";

        public Expression CreateExpression(Expression context, Expression argumentsRoot, IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            var a = context;
            var b = arguments[0];

            if (a.Type != b.Type)
                b = Expression.Convert(b, a.Type);

            return Expression.Equal(a, b);
        }

        public Expression GetArgumentsRoot(Expression context)
        {
            return context;
        }
    }
}
