using QRest.Core.Contracts;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace QRest.Core.Operations.Query
{
    public class WhereOperation : LambdaOperationBase
    {
        public override string Key { get; } = "where";

        protected override Expression CreateExpression(ParameterExpression root, Expression context, Type element, Type collectionType, LambdaExpression argument, IAssemblerContext assembler)
        {
            var exp = Expression.Call(collectionType, nameof(Queryable.Where), new Type[] { element }, context, argument);
            return assembler.SetName(exp, "data");
        }       
    }
}
