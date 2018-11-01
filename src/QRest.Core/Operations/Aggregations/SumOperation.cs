using QRest.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Aggregations
{
    public class SumOperation : OperationBase
    {
        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            var lambda = (LambdaExpression)arguments[0];

            var exp = Expression.Call(typeof(Queryable), nameof(Queryable.Sum), new Type[] { lambda.Parameters[0].Type }, context, lambda);

            return new NamedExpression(exp, nameof(Queryable.Sum));
        }        
    }
}
