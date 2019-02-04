using QRest.Core.Compilation;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Selectors
{
    public sealed class RootOperation : OperationBase
    {
        internal RootOperation() { }

        public override string Key { get; } = "$";

        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssembler assembler)
        {
            return root;
        }
    }
}
