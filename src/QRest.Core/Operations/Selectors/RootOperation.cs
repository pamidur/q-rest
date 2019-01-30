using QRest.Core.Contracts;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public sealed class RootOperation : OperationBase
    {
        internal RootOperation() { }

        public override string Key { get; } = "$";

        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            return root;
        }
    }
}
