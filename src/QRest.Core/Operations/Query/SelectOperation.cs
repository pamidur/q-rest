using QRest.Core.Contracts;
using QRest.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    public class SelectOperation : OperationBase
    {
        public override string Key { get; } = "select";

        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            var queryElement = context.GetQueryElementType();

            var lambda = (LambdaExpression)arguments[0];
            var param = lambda.Parameters[0];

            var resultExpression = Expression.Call(typeof(Queryable), nameof(Queryable.Select)
                , new Type[] { queryElement, lambda.ReturnType }, context, lambda);

            return resultExpression;
        }
    }
}
