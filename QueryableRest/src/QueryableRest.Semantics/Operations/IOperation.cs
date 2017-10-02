using System.Collections.Generic;
using System.Linq.Expressions;

namespace QueryableRest.Semantics.Operations
{
    public interface IOperation
    {
        IReadOnlyCollection<string> Monikers { get; }
        Expression CreateExpression(IReadOnlyList<Expression> arguments);
    }
}
