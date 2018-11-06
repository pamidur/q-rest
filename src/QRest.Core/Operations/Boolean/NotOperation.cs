using QRest.Core.Contracts;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public class NotOperation : OperationBase
    {
        public override string Key { get; } = "not";

        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            if (arguments.Count != 0)
                throw new ExpressionCreationException();

            return Expression.Not(context);
        }
    }
}
