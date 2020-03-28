using QRest.Core.Compilation;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public sealed class HasOperation : OperationBase
    {
        internal HasOperation() { }

        public override string Key { get; } = "has";

        public override Expression CreateExpression(Expression context, IReadOnlyList<Expression> arguments, IAssembler assembler)
        {
            if (arguments.Count != 1)
                throw new CompilationException("Expected 1 parameter");

            if (context.Type == typeof(string))
            {
                return Expression.Call(context, nameof(String.Contains), new Type[] { }, arguments[0]);
            }

            throw new CompilationException("Expected string context");
        }
    }
}
