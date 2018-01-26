using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations
{
    public abstract class OperationBase : IOperation
    {
        public abstract Expression CreateExpression(Expression last, ParameterExpression root, IReadOnlyList<Expression> arguments, QueryContext context);

        public override string ToString()
        {
            return GetType().Name.ToLowerInvariant().Replace("operation", "");
        }
    }
}
