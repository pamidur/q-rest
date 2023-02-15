using QRest.Core.Compilation;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public abstract class OperationBase : IOperation
    {
        public abstract string Key { get; }
        public override string ToString() => Key;
        public abstract Expression CreateExpression(Expression context, IReadOnlyList<Expression> arguments, IAssembler assembler);
    }
}
