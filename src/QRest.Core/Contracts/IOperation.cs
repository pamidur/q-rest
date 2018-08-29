using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public interface IOperation
    {
        bool SupportsQuery { get; }
        bool SupportsCall{ get; }

        Expression CreateQueryExpression(ParameterExpression root, Expression context, ParameterExpression argumentsRoot, IReadOnlyList<Expression> arguments);
        Expression CreateCallExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments);
    }
}
