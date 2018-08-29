using QRest.Core.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    public class SkipOperation : OperationBase
    {
        public override bool SupportsQuery => true;

        public override Expression CreateQueryExpression(ParameterExpression root, Expression context, ParameterExpression argumentsRoot, IReadOnlyList<Expression> arguments)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            if (!TryCast(arguments[0], typeof(Int32), out var argument))
                throw new ExpressionCreationException($"Cannot cast {arguments[0].Type} to Int32");

            var exp =  Expression.Call(typeof(Queryable), nameof(Queryable.Skip), new Type[] { argumentsRoot.Type }, context, argument);

            return new NamedExpression(exp, NamedExpression.DefaultQueryResultName);
        }       
    }
}
