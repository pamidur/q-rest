using QRest.Core.Contracts;
using QRest.Core.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Aggregations
{
    public class FirstOperation : QueryOperationBase
    {
        public override string Key { get; } = "first";

        protected override Expression CreateExpression(ParameterExpression root, Expression context, Type element, bool queryable, IReadOnlyList<Expression> arguments, IAssemblerContext assembler)
        {
            if (arguments.Count != 0)
                throw new TermTreeCompilationException($"Method '{Key}' expects to have no parameters.");

            var exp = Expression.Call(typeof(Queryable), nameof(Queryable.FirstOrDefault), new Type[] { element }, context);

            return assembler.SetName(exp, Key);
        }
    }
}
