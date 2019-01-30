using QRest.Core.Contracts;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    public sealed class MapOperation : LambdaOperationBase
    {
        internal MapOperation() { }

        public override string Key { get; } = "map";

        protected override Expression CreateExpression(ParameterExpression root, Expression context, Type element, LambdaExpression argument, IAssemblerContext assembler)
        {
            var exp = Expression.Call(QueryableType, nameof(Queryable.Select)
                , new Type[] { element, argument.ReturnType }, context, argument);

            return assembler.SetName(exp, "data");
        }
    }
}
