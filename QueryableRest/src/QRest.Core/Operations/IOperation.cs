using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public interface IOperation
    {
        Expression CreateExpression(Expression last, ParameterExpression root, IReadOnlyList<Expression> arguments, QueryContext context);
    }
}
