using QRest.Core.Contracts;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Aggregations
{
    public class AnyOperation : LambdaOperationBase
    {
        public override string Key { get; } = "any";

        protected override Expression CreateExpression(ParameterExpression root, Expression context, Type element, LambdaExpression argument, IAssemblerContext assembler)
        {
            var exp = (Expression)Expression.Call(QueryableType, nameof(Queryable.Any), new Type[] { element }, new[] { context, argument });
            return assembler.SetName(exp, Key);
        }
    }
}
