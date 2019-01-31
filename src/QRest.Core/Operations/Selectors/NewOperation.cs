using QRest.Core.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Selectors
{
    public sealed class NewOperation : OperationBase
    {
        internal NewOperation() { }

        public override string Key { get; } = "new";

        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            var expression = arguments.Any() ? assembler.CreateContainer(arguments) : context;
            return expression;
        }
    }
}
