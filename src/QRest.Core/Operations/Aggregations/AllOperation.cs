using QRest.Core.Contracts;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Aggregations
{
    public class AllOperation : LambdaOperationBase
    {
        public override string Key { get; } = "all";

        protected override Expression CreateExpression(ParameterExpression root, Expression context, Type element, Type collection, LambdaExpression argument, IAssemblerContext assembler)
        {          
            var exp = (Expression) Expression.Call(collection, nameof(Queryable.All), new Type[] { element }, new[] { context, argument });
            return assembler.SetName(exp, Key);
        }
    }
}
