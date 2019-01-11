using QRest.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Boolean
{
    public class HasOperation : OperationBase
    {
        public override string Key { get; } = "has";

        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            if (context.Type == typeof(string))
            {
                return Expression.Call(context, nameof(String.Contains), new Type[] { }, arguments[0]);
            }

            throw new ExpressionCreationException();
        }
    }
}
