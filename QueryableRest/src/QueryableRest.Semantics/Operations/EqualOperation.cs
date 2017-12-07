using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public class EqualOperation : IOperation
    {
        public Expression CreateExpression(Expression context, ParameterExpression root, IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            var a = context;
            var b = arguments[0];

            if (a.Type != b.Type)
            {
                if (a.Type.IsAssignableFrom(b.Type))
                    a = Expression.Convert(a, b.Type);
                else if (b.Type.IsAssignableFrom(a.Type))
                    b = Expression.Convert(b, a.Type);
                else throw new ExpressionCreationException();
            }

            return Expression.Equal(a, b);
        }
    }
}
