using QRest.Core.Compilation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Aggregations
{
    public sealed class SumOperation : QueryOperationBase
    {
        internal SumOperation() { }

        public override string Key { get; } = "sum";

        protected override Expression CreateExpression(Expression context, Type collection, Type element, IReadOnlyList<Expression> arguments, IAssembler assembler)
        {
            if (arguments.Count != 0)
                throw new CompilationException($"Method '{Key}' expects to have no parameters.");

            var exp = Expression.Call(collection, nameof(Queryable.Sum), new Type[] { }, context);

            return assembler.SetName(exp, Key);
        }
    }
}
