using QRest.Core.Contracts;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Selectors
{
    public class ContextOperation : OperationBase
    {
        public override string Key { get; } = "$$";

        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            return context;
        }
    }
}
