using QRest.Core.Compilation;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public sealed class NotOperation : OperationBase
    {
        internal NotOperation() { }

        public override string Key { get; } = "not";

        public override Expression CreateExpression(Expression context, IReadOnlyList<Expression> arguments, IAssembler assembler)
        {
            if (arguments.Count != 0)
                throw new CompilationException("Expected 0 parameters");

            return Expression.Not(context);
        }
    }
}
