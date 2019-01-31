using QRest.Core.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public sealed class OneOfOperation : OperationBase
    {
        internal OneOfOperation() { }

        public override string Key { get; } = "oneof";

        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            if (arguments.Count < 1)
                throw new ExpressionCreationException();

            var body = arguments.Aggregate((e1, e2) => { return Expression.OrElse(e1, e2); });

            return body;
        }
    }
}
