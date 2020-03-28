using QRest.Core.Compilation;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public sealed class EveryOperation : OperationBase
    {
        internal EveryOperation() { }

        public override string Key { get; } = "every";

        public override Expression CreateExpression(Expression context, IReadOnlyList<Expression> arguments, IAssembler assembler)
        {
            if (arguments.Count < 1)
                throw new CompilationException("Expected 1 or more parameters");

            var body = arguments.Aggregate((e1, e2) => { return Expression.AndAlso(e1, e2); });

            return body;
        }
    }
}
