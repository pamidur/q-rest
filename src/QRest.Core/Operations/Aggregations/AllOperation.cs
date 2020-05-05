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

        protected override Expression CreateExpression(Expression context, Type collection, Type element, LambdaExpression argument, IAssembler assembler)
        {
            var exp = (Expression)Expression.Call(collection, nameof(Queryable.All), new Type[] { element }, new[] { context, argument });
            return assembler.SetName(exp, Key);
        }
    }
}
