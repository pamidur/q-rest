using QRest.Core.Contracts;
using QRest.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Aggregations
{
    public class AnyOperation : OperationBase
    {
        public override string Key { get; } = "any";

        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            var args = new List<Expression>
            {
                context
            };

            if(arguments.Count==1)
            {
                if (arguments[0].NodeType != ExpressionType.Lambda)
                    throw new ExpressionCreationException();

                args.Add(arguments[0]);
            }

            if (arguments.Count > 1)
                throw new ExpressionCreationException();

            var exp = Expression.Call(typeof(Queryable), nameof(Queryable.Any), new Type[] { context.GetQueryElementType() }, args.ToArray());

            return exp;
        }      
    }
}
