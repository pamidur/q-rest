using System.Collections.Generic;
using System.Linq.Expressions;

namespace QueryableRest.Semantics.Operations
{
    public interface IOperation
    {
        Expression CreateExpression(Expression context, Expression argumentsRoot, IReadOnlyList<Expression> arguments);
        Expression GetArgumentsRoot(Expression context);
    }
}
