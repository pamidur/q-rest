using QRest.Core.Contracts;
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
        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            var exp = context;

            foreach (var arg in arguments)
            {
                var lambda = (LambdaExpression)arg;

                var reduced = arg.ReduceTo(new[] { AscendingExpression.AscendingExpressionType, DescendingExpression.DescendingExpressionType });

                var method = nameof(Queryable.OrderBy);
                if (reduced.NodeType == DescendingExpression.DescendingExpressionType)
                    method = nameof(Queryable.OrderByDescending);

                exp = Expression.Call(typeof(Queryable), method, new Type[] { lambda.Parameters[0].Type, reduced.Type }, exp, lambda);
            }

            return new NamedExpression(exp, NamedExpression.DefaultQueryResultName);
        }
    }
}
