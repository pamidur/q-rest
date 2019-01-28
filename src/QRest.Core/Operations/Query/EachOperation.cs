using QRest.Core.Contracts;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    public class EachOperation : LambdaOperationBase
    {
        public override string Key { get; } = "each";

        protected override Expression CreateExpression(ParameterExpression root, Expression context, Type element, bool queryable, LambdaExpression argument, IAssemblerContext assembler)
        {
            var exp = Expression.Call(typeof(Queryable), nameof(Queryable.Select)
                , new Type[] { element, argument.ReturnType }, context, argument);

            return assembler.SetName(exp, "data");
        }
    }
}
