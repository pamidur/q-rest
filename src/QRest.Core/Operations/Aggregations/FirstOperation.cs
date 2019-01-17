using QRest.Core.Contracts;
using QRest.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Aggregations
{
    public class FirstOperation : OperationBase
    {
        public override string Key { get; } = "first";

        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            if (arguments.Count != 0)
                throw new ExpressionCreationException();

            var exp = Expression.Call(typeof(Queryable), nameof(Queryable.FirstOrDefault), new Type[] { context.GetQueryElementType() }, context);

            return assembler.SetName(exp, Key);
        }
    }
}
