using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace QRest.Core.Operations
{
    public class SelfOperation : IOperation
    {
        public Expression CreateExpression(Expression last, ParameterExpression root, IReadOnlyList<Expression> arguments, QueryContext context)
        {
            return last;
        }
    }
}
