using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public class WithOperation : IOperation
    {
        public Expression CreateExpression(Expression last, ParameterExpression root, IReadOnlyList<Expression> arguments, QueryContext context)
        {
            return last;
        }
    }
}
