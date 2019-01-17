using QRest.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    public class WhereOperation : OperationBase
    {
        public override string Key { get; } = "where";

        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            if (arguments[0].NodeType != ExpressionType.Lambda)
                throw new ExpressionCreationException();

            var lambda = (LambdaExpression)arguments[0];

            var exp = Expression.Call(typeof(Queryable), nameof(Queryable.Where), new Type[] { lambda.Parameters[0].Type }, context, lambda);

            return assembler.SetName(exp, "data");
        }      
    }
}
