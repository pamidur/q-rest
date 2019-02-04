using QRest.Core.Compilation;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Aggregations
{
    public sealed class AllOperation : LambdaOperationBase
    {
        internal AllOperation() { }

        public override string Key { get; } = "all";

        protected override Expression CreateExpression(ParameterExpression root, Expression context, Type element, LambdaExpression argument, IAssembler assembler)
        {
            var exp = (Expression)Expression.Call(QueryableType, nameof(Queryable.All), new Type[] { element }, new[] { context, argument });
            return assembler.SetName(exp, Key);
        }
    }
}
