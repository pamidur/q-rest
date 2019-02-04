using QRest.Core.Compilation;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public interface IOperation
    {
        Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssembler assembler);
        string Key { get; }
    }
}
