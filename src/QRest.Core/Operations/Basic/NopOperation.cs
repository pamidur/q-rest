using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Basic
{
    public class NopOperation : OperationBase
    {
        public override Expression CreateCallExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            return context;
        }
    }
}
