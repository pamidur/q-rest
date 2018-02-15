using QRest.Core.Expressions;
using QRest.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    public class OrderOperation : OperationBase
    {
        public override bool SupportsQuery => true;

        public override Expression CreateQueryExpression(ParameterExpression root, Expression context, ParameterExpression argumentsRoot, IReadOnlyList<Expression> arguments)
        {
            var exp = context;

            foreach (var arg in arguments)
            {
                var lambda = Expression.Lambda(arg, argumentsRoot);

                var reduced = arg.ReduceTo(new[] { AscendingExpression.AscendingExpressionType, DescendingExpression.DescendingExpressionType });

                var method = nameof(Queryable.OrderBy);
                if (reduced.NodeType == DescendingExpression.DescendingExpressionType)
                    method = nameof(Queryable.OrderByDescending);

                exp = Expression.Call(typeof(Queryable), method, new Type[] { argumentsRoot.Type, reduced.Type }, exp, lambda);
            }

            return new NamedExpression(exp, NamedExpression.DefaultQueryResultName);
        }
    }
}
