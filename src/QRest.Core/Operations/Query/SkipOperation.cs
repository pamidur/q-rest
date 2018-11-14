using QRest.Core.Contracts;
using QRest.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    public class SkipOperation : OperationBase
    {
        public override string Key { get; } = "skip";

        public override Expression CreateExpression(ParameterExpression root, Expression context, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            if (!assembler.TryConvert(arguments[0], typeof(int), out var argument))
                throw new ExpressionCreationException($"Cannot cast {arguments[0].Type} to Int32");

            var exp =  Expression.Call(typeof(Queryable), nameof(Queryable.Skip), new Type[] { context.GetQueryElementType() }, context, argument);

            return exp;
        }       
    }
}
