using QRest.Core.Compilation;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    public sealed class MapOperation : LambdaOperationBase
    {
        internal MapOperation() { }

        public override string Key { get; } = "map";

        protected override Expression CreateExpression(Expression context, Type collection, Type element, LambdaExpression argument, IAssembler assembler)
        {
            var exp = Expression.Call(collection, nameof(Queryable.Select)
                , new Type[] { element, argument.ReturnType }, context, argument);

            return assembler.SetName(exp, "data");
        }
    }
}
