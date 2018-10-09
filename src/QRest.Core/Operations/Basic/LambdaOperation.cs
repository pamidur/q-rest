using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace QRest.Core.Operations.Basic
{
    public class LambdaOperation : OperationBase
    {
        public override Expression CreateCallExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            return base.CreateCallExpression(root, context, arguments);
        }
    }
}
