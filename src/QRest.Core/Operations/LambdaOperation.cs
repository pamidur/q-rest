using QRest.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace QRest.Core.Operations
{
    public class LambdaOperation : IOperation
    {
        public bool SupportsQuery => throw new NotImplementedException();

        public bool SupportsCall => throw new NotImplementedException();

        public Expression CreateCallExpression(Expression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            throw new NotImplementedException();
        }

        public Expression CreateQueryExpression(Expression context, ParameterExpression argumentsRoot, IReadOnlyList<Expression> arguments)
        {
            throw new NotImplementedException();
        }
    }
}
