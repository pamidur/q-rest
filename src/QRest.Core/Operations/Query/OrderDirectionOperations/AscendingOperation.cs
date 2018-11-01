using QRest.Core.Contracts;
using QRest.Core.Expressions;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query.OrderDirectionOperations
{
    public class AscendingOperation : OperationBase
    {
        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            if (arguments.Count != 0)
                throw new ExpressionCreationException();

            return new AscendingExpression(context);
        }
    }
}
