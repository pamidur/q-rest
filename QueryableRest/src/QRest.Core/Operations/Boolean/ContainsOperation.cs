using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace QRest.Core.Operations.Boolean
{
    public class ContainsOperation : IOperation
    {
        public Expression CreateExpression(Expression last, ParameterExpression root, IReadOnlyList<Expression> arguments, QueryContext context)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            if(last.Type == typeof(string))
            {
                return Expression.Call(last, "Contains", new Type[] { }, arguments[0]);
            }

            throw new Exception();
        }
    }
}
