using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Basic
{
    public class LambdaOperation : OperationBase
    {
        public override Expression CreateCallExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            var lambda = Expression.Lambda(arguments[0], root);
            return lambda;
        }
    }
}
