using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Contracts
{
    public interface IOperation
    {
        bool SupportsQuery { get; }
        bool SupportsCall{ get; }

        Expression CreateQueryExpression(Expression context, ParameterExpression argumentsRoot, IReadOnlyList<Expression> arguments);
        Expression CreateCallExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments);

        
    }
}
