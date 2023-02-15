using QRest.Core.Compilation;
using QRest.Core.Compilation.Expressions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    internal class ReverseOrderExpression : ProxyExpression
    {
        public ReverseOrderExpression(Expression expression) : base(expression) { }
    }

    public sealed class OrderOperation : QueryOperationBase
    {
        internal OrderOperation() { }

        public override string Key { get; } = "order";

        protected override Expression CreateExpression(Expression context, Type collection, Type element, IReadOnlyList<Expression> arguments, IAssembler assembler)
        {
            var exp = context;

            var beginning = true;

            foreach (var arg in arguments)
            {
                var lambda = (LambdaExpression)arg;

                var reversed = lambda.Body is ReverseOrderExpression;

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

                exp = Expression.Call(collection, method, new Type[] { lambda.Parameters[0].Type, lambda.ReturnType }, exp, lambda);
            }

            return assembler.SetName(exp, "data");
        }        
    }

    public sealed class ReverseOrderOperation : OperationBase
    {
        internal ReverseOrderOperation() { }

        public override string Key { get; } = "desc";

        public override Expression CreateExpression(Expression context, IReadOnlyList<Expression> arguments, IAssembler assembler)
        {
            if (arguments.Count != 0)
                throw new CompilationException("Expected 0 parameters");

            return new ReverseOrderExpression(context);
        }
    }
}
