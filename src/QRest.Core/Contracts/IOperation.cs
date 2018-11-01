using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Contracts
{
    public interface IOperation
    {
        Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments);
        string Key { get; }
    }
}
