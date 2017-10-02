using System.Collections.Generic;
using System.Linq.Expressions;

namespace QueryableRest.Semantics.Operations
{
    public class EqualOperation : IOperation
    {
        public Expression CreateExpression(IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count != 2)
                throw new ExpressionCreationException();

            var a = arguments[0];
            var b = arguments[1];

            if (a.Type != b.Type)
                b = Expression.Convert(b, a.Type);

            return Expression.Equal(a, b);
        }
    }
}
