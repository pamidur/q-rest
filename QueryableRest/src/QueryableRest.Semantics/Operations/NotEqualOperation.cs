using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public class NotEqualOperation : IOperation
    {
        public Expression CreateExpression(Expression context, ParameterExpression root, IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            var a = context;
            var b = arguments[0];

            if (a.Type != b.Type)
                b = Expression.Convert(b, a.Type);

            return Expression.NotEqual(a, b);
        }
    }
}
