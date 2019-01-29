using QRest.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    public class TakeOperation : QueryOperationBase
    {      
        public override string Key { get; } = "take";

        protected override Expression CreateExpression(ParameterExpression root, Expression context, Type element, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            if (arguments.Count != 1)
                throw new ExpressionCreationException();

            if (!assembler.TryConvert(arguments[0], typeof(int), out var argument))
                throw new ExpressionCreationException($"Cannot cast {arguments[0].Type} to Int32");

            var exp = Expression.Call(QueryableType, nameof(Queryable.Take), new Type[] { element }, context, argument);

            return assembler.SetName(exp, "data");
        }
    }
}
