using QRest.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    class ReverseOrderExpression : ProxyExpression
    {
        public static readonly ExpressionType ReverseOrderNodeType = (ExpressionType)1100;
        public ReverseOrderExpression(Expression expression) : base(expression, ReverseOrderNodeType) { }
    }

    public class OrderOperation : OperationBase
    {
        public override string Key { get; } = "order";

        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            var exp = context;

            var beginning = true;

            foreach (var arg in arguments)
            {
                var lambda = (LambdaExpression)arg;

                var reversed = lambda.Body.NodeType == ReverseOrderExpression.ReverseOrderNodeType;

                var method = nameof(Queryable.OrderBy);

                if (beginning)
                {
                    if (reversed)
                        method = nameof(Queryable.OrderByDescending);
                    beginning = false;
                }
                else
                {
                    method = nameof(Queryable.ThenBy);
                    if (reversed)
                        method = nameof(Queryable.ThenByDescending);
                }

                exp = Expression.Call(typeof(Queryable), method, new Type[] { lambda.Parameters[0].Type, lambda.ReturnType }, exp, lambda);
            }

            return exp;
        } 
    }

    public class ReverseOrderOperation : OperationBase
    {
        public override string Key { get; } = "desc";

        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            if (arguments.Count != 0)
                throw new ExpressionCreationException();

            return new ReverseOrderExpression(context);
        }
    }
}
