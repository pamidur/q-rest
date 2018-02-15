using QRest.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    public class WhereOperation : OperationBase
    {
        public override bool SupportsQuery => true;

        public override Expression CreateQueryExpression(ParameterExpression root, Expression context, ParameterExpression argumentsRoot, IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            if (arguments[0].Type != typeof(bool))
                throw new ExpressionCreationException();

            var lambda = Expression.Lambda(arguments[0], argumentsRoot);

            var exp = Expression.Call(typeof(Queryable), nameof(Queryable.Where), new Type[] { argumentsRoot.Type }, context, lambda);

            return new NamedExpression(exp, NamedExpression.DefaultQueryResultName);
        }       
    }
}
